using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.DAL;
using MiOU.Entities;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Util;
namespace MiOU.BL
{
    public class OrderManagement:BaseManager
    {
        public OrderManagement(int userId) : base(userId)
        {
        }

        public OrderManagement(BUser user) : base(user)
        {

        }
        protected string GetOrderRandomNumber()
        {
            Random rad = new Random();//实例化随机数产生器rad；
            int value = rad.Next(1000, 10000);//用rad生成大于等于1000，小于等于9999的随机数；
            return value.ToString();
        }

        public List<BOrder> SearchOrders()
        {
            List<BOrder> orders = null;
            return orders;
        }

        /// <summary>
        /// Create new order and return the order no of new created order
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="priceCategory"></param>
        /// <param name="description"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="contact"></param>
        /// <param name="phone"></param>
        /// <param name="deliveryType"></param>
        /// <param name="rentType"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <param name="address"></param>
        /// <param name="payType">Payment type, online payment, offline payment etc...</param>
        /// <returns>New Order Number</returns>
        public BOrder CreateOrder(int productId,int priceCategory,string description,DateTime? startDate,DateTime? endDate,string contact,string phone,int deliveryType,int rentType,int province,int city,int district,string address,int payType)
        {
            string orderNo = null;
            BOrder order = null;
            if (productId<=0)
            {
                throw new MiOUException("请选择产品");
            }
            if (priceCategory <= 0)
            {
                throw new MiOUException("请选择产品价格类别");
            }
            if(rentType<=0)
            {
                throw new MiOUException("请选择租赁付费类别");
            }
            if (deliveryType <= 0)
            {
                throw new MiOUException("请选择交付类别");
            }
            if (startDate==null)
            {
                throw new MiOUException("请选择产品租赁起始时间");
            }
            if (endDate == null)
            {
                throw new MiOUException("请选择产品租赁结束时间");
            }
            if(province<=0)
            {
                throw new MiOUException("请填写租赁人所在省份");
            }
            if (city <= 0)
            {
                throw new MiOUException("请填写租赁人所在城市");
            }
            if (string.IsNullOrEmpty(address))
            {
                throw new MiOUException("请填写租赁人详细地址");
            }
            if (string.IsNullOrEmpty(contact))
            {
                throw new MiOUException("请填写联系人");
            }
            if (string.IsNullOrEmpty(phone))
            {
                throw new MiOUException("请填写联系电话");
            }
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                Product dbProduct = (from p in db.Product where p.Id==productId select p).FirstOrDefault<Product>();
                if(dbProduct==null)
                {
                    throw new MiOUException("产品不存在");
                }
                if(dbProduct.Locked)
                {
                    throw new MiOUException("此产品被锁住，不能租借");
                }
                if(dbProduct.Repertory<1)
                {
                    throw new MiOUException("此产品库存不足不能租借");
                }
                User owner = (from u in db.User where u.Id == dbProduct.UserId select u).FirstOrDefault<User>();
                if(owner==null)
                {
                    throw new MiOUException("此产品的所有人不存在");
                }
                if(owner.Status==0)
                {
                    throw new MiOUException("此产品的所有人被禁用，此人的产品不能被借用");
                }
                ProductPrice price = (from pp in db.ProductPrice where pp.PriceCategory==priceCategory select pp).FirstOrDefault<ProductPrice>();
                if(price==null)
                {
                    throw new MiOUException("请选正确的产品价格类别");
                }

                if(rentType==2)
                {
                    List<VipLevel> vips = (from uv in db.UserVip
                                          join v in db.VipLevel on uv.VipLevelId equals v.Id into lv
                                          from llv in lv.DefaultIfEmpty()
                                          orderby uv.CurrencyAmount descending
                                          select llv).ToList<VipLevel>();

                    if(vips.Count<=0)
                    {
                        throw new MiOUException("只有VIP用户才可以选择VIP借用的方式租赁，请先兑换VIP等级");
                    }

                    VipLevel topVip = vips[0];
                    float productValue = dbProduct.EvaluatedPrice * dbProduct.EvaluatedPercentage;
                    if (productValue>topVip.End)
                    {
                        throw new MiOUException("VIP等级过低，不能使用VIP借用租赁此产品");
                    }
                }
                Order newOrder = new Order()
                {
                    ContactName = contact,
                    ContactPhone = phone,
                    Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                    Description = description,
                    EndTime = endDate != null ? DateTimeUtil.ConvertDateTimeToInt((DateTime)endDate) : 0,
                    PriceCategory = priceCategory,
                    PriceId = price.Id,
                    ProductId = productId,
                    StartTime = startDate != null ? DateTimeUtil.ConvertDateTimeToInt((DateTime)startDate) : 0,
                    Status = 0,
                    Updated = 0,
                    UpdatedUserId = 0,
                    UserId = CurrentLoginUser.User.Id,
                    OrderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + GetOrderRandomNumber(),                    
                };

                db.Order.Add(newOrder);
                db.SaveChanges();
                if(newOrder.Id<=0)
                {
                    logger.Error("Failed to create new order.");
                }
                else
                {
                    orderNo = newOrder.OrderNo;
                    //going to created order payment record
                    order = new BOrder()
                    {
                         ContactName= contact,
                         ContactPhone=phone,
                         Created=newOrder.Created,
                         CreatedBy= new BUser() { User= CurrentLoginUser.User },
                         Id= newOrder.Id,
                         Description=description,
                         EndTime=order.EndTime,
                          StartTime=order.StartTime,
                         EPrice=new BEvaluatedPrice() { },
                         Name= order.Name,
                         OrderNo=newOrder.OrderNo,
                         Payments=new List<BOrderPayment>(),
                         PriceCategory= new BPriceCategory() {Id= priceCategory },
                         Product= new BProduct() { Id= productId },
                         Status=(OrderStatus)newOrder.Status,
                         Updated=0,
                         UpdatedBy=null
                    };

                    PaymentManagement payMgt = new PaymentManagement(CurrentLoginUser);
                    payMgt.GeneratePaymentRecord(ref order);
                }
            }
            catch(MiOUException mex)
            {
                logger.Error(mex);
                throw mex;
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }

            return order;
        }
    }
}
