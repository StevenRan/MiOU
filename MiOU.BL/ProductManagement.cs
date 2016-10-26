using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.DAL;
using MiOU.Entities.Beans;
using MiOU.Entities.Exceptions;
using MiOU.Util;
using MiOU.Entities;
namespace MiOU.BL
{
    public class ProductManagement:BaseManager
    {
        private static object obj = new object();
        public ProductManagement(int userId) : base(userId)
        {
        }

        public ProductManagement(BUser user) : base(user)
        {

        }

        public void DeleteProductImages(int productId,List<int> images)
        {
            if(productId<=0 || images==null || images.Count<=0)
            {
                return;
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                Product product = (from pdt in db.Product where pdt.Id == productId select pdt).FirstOrDefault<Product>();
                if (product == null)
                {
                    throw new MiOUException("产品信息不存在");
                }
                bool delete = false;
                if(product.UserId!=CurrentLoginUser.User.UserId)
                {
                    if(CurrentLoginUser.IsAdmin && CurrentLoginUser.Permission.DELETE_PRODUCT_IMAGES)
                    {
                        delete = true;
                    }
                    else
                    {
                        throw new MiOUException("只有管理员和产品所有者才能删除产品图片");
                    }
                }
                else
                {
                    delete = true;

                }
                
                if(delete)
                {
                    int[] ids = images.ToArray<int>();
                    List<MiOU.DAL.File> files = (from a in db.File where ids.Contains(a.Id) select a).ToList<MiOU.DAL.File>();
                    List<ProductImage> productImages = (from p in db.ProductImage where ids.Contains(p.ImageId) && p.ProductId==productId select p).ToList<ProductImage>();
                    int[] fileIds = (from pi in productImages select pi.ImageId).ToArray<int>();
                    if(fileIds!=null && fileIds.Length>0)
                    {
                        List<MiOU.DAL.File> toDeletedFiles = (from f in files where fileIds.Contains(f.Id) select f).ToList<MiOU.DAL.File>();
                        if(toDeletedFiles.Count>0)
                        {
                            foreach(MiOU.DAL.File f in toDeletedFiles)
                            {
                                string fullFilePath = System.IO.Path.Combine(WebSitePhysicalDirectory, f.Path.Replace("/", "\\"));
                                if(System.IO.File.Exists(fullFilePath))
                                {
                                    System.IO.File.Delete(fullFilePath);
                                    if (!System.IO.File.Exists(fullFilePath))
                                    {
                                        db.File.Remove(f);
                                    }
                                } 
                            }                            
                        }
                    }
                }
            }
        }

        public bool AuditProduct(int productId, int status, string message,float ePrice,float percentage)
        {
            bool ret = false;
            if (string.IsNullOrEmpty(message))
            {
                throw new MiOUException("审核信息不能为空");
            }
            if (CurrentLoginUser == null || CurrentLoginUser.User == null)
            {
                throw new MiOUException("请先登录");
            }
            if (CurrentLoginUser.Permission == null)
            {
                throw new MiOUException("没有权限进行此操作");
            }
            if (!CurrentLoginUser.IsAdmin)
            {
                throw new MiOUException("非管理员用户，没有权限进行此操作");
            }
            if (!CurrentLoginUser.Permission.SHENHE_PRODUCT)
            {
                throw new MiOUException("没有权限审核产品");
            }

            using (MiOUEntities db = new MiOUEntities())
            {
                Product product = (from pdt in db.Product where pdt.Id == productId select pdt).FirstOrDefault<Product>();
                if (product == null)
                {
                    throw new MiOUException("产品信息不存在");
                }
                if (product.Status == status && status != 0)
                {
                    throw new MiOUException("不能进行同样结果的审核");
                }

                product.Status = status;
                product.AuditMessage = message;
                product.AuditTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                product.AuditUserId = CurrentLoginUser.User.UserId;
                product.EvaluatedPercentage = percentage;
                product.EvaluatedPrice = ePrice;
                db.SaveChanges();
                GenerateProductPrice(product, db);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// 租赁出去 count未负整数，归还count为正整数
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="orderId"></param>
        /// <param name="count"></param>
        public void UpadteProductRepertory(int productId,int orderId,int count)
        {
            MiOUEntities db = null;
            try
            {              

                lock(obj)
                {
                    db = new MiOUEntities();
                    Product dbProduct = (from p in db.Product where p.Id == productId select p).FirstOrDefault<Product>();
                    if (dbProduct == null)
                    {
                        throw new MiOUException("产品不存在");
                    }
                    if (orderId>0)
                    {
                        Order order = (from o in db.Order where o.Id == orderId select o).FirstOrDefault<Order>();
                        if (order.ProductId != productId)
                        {
                            throw new MiOUException("订单的产品和要更新库存的产品不匹配");
                        }

                        if (order.UserId != CurrentLoginUser.User.UserId && dbProduct.UserId!=CurrentLoginUser.User.UserId && (!CurrentLoginUser.IsAdmin))
                        {
                            throw new MiOUException("没有权限执行此操作");
                        }
                    }
                    else
                    {
                        if(dbProduct.UserId!=CurrentLoginUser.User.UserId)
                        {
                            throw new MiOUException("没有权限执行此操作");
                        }
                    }

                   
                    dbProduct.Repertory = dbProduct.Repertory + count;
                    db.SaveChanges();   
                }
            }
            catch(MiOUException mex)
            {
                logger.Error(mex);
                throw mex;
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// After the product is audited, the product prices should be automatically generated
        /// </summary>
        /// <param name="product"></param>
        /// <param name="db"></param>
        private void GenerateProductPrice(Product product,MiOUEntities db)
        {
            List<ProductPrice> prices = (from p in db.ProductPrice where p.EvaluatedPriceId==0 select p).ToList<ProductPrice>();
            if(prices.Count==0)
            {
                return;
            }

            int[] priceCategories = (from cate in prices select cate.PriceCategory).ToArray<int>();
            float rPrice = product.EvaluatedPrice * product.EvaluatedPercentage;
            int ePriceCatge = (from epc in db.EvaluatedPriceCategory where epc.StartPrice <= rPrice && epc.EndPrice<rPrice select epc.Id).FirstOrDefault<int>();
            if(ePriceCatge==0)
            {
                return;
            }

            List<EvaluatedPrice> eprices = (from eprice in db.EvaluatedPrice where eprice.EvaluatedPriceCategory==ePriceCatge && priceCategories.Contains(eprice.PriceCategory) select eprice).ToList<EvaluatedPrice>();
            foreach(EvaluatedPrice eprice in eprices)
            {
                ProductPrice price = (from pdtPrice in prices where pdtPrice.ProductId == product.Id && pdtPrice.PriceCategory == eprice.PriceCategory select pdtPrice).FirstOrDefault<ProductPrice>();
                if (price != null)
                {
                    price.EvaluatedPriceId = eprice.Id;
                    price.Price = eprice.Price;                  
                }
            }
            db.SaveChanges();
        }

        public void CreateProduct(BProduct model)
        {
            if(model==null)
            {
                throw new MiOUException("产品数据不正确");
            }
            if(model.Category==null || model.Category.Id<=0)
            {
                throw new MiOUException("产品类别不能为空");
            }
            if(string.IsNullOrEmpty(model.Name))
            {
                throw new MiOUException("产品名称不能为空");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                throw new MiOUException("产品描述不能为空");
            }
            if(string.IsNullOrEmpty(model.Address))
            {
                throw new MiOUException("产品地址不能为空");
            }
            if(model.DeliveryType==null || model.DeliveryType.Id<=0)
            {
                throw new MiOUException("产品交付方式不能为空");
            }
            if(model.RentType==null || model.RentType.Id<=0)
            {
                throw new MiOUException("付费类型不能为空");
            }
            if(model.Province==null || model.Province.Id<=0)
            {
                throw new MiOUException("省份不能为空");
            }
            if (model.City == null || model.City.Id <= 0)
            {
                throw new MiOUException("城市份不能为空");
            }
            if (model.District== null || model.District.Id <= 0)
            {
                throw new MiOUException("区县不能为空");
            }
            if(model.Percentage==0)
            {
                throw new MiOUException("产品成色不能为空");
            }
            if(model.Images==null)
            {
                throw new MiOUException("缺少产品图片");
            }
            if (model.Images.Count<3)
            {
                throw new MiOUException("产品图片至少三张");
            }
            if(model.ProductPrices==null || model.ProductPrices.Count<=0)
            {
                throw new MiOUException("产品的租赁租价方式至少一种");
            }
            if(CurrentLoginUser.User.UserType==1 && model.Repertory>1)
            {
                throw new MiOUException("个人用户的藕品库存必须等于1，如有同种产品请新添加一个藕品");
            }
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                Product product = new Product()
                {
                    Address = model.Address,
                    AuditMessage = "",
                    AuditTime = 0,
                    AuditUserId = 0,
                    CategoryId = model.Category.Id,
                    Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                    City = model.City.Id,
                    DeliveryType = model.DeliveryType.Id,
                    Description = model.Description,
                    District = model.District.Id,
                    Percentage = model.Percentage,
                    Province = model.Province.Id,
                    RentType = model.RentType.Id,
                    Status = 0,
                    UserId = CurrentLoginUser.User.UserId,
                    Pledge = 0,
                    Price = 0,
                    Updated = 0,
                    XPlot = "",
                    YPlot = "",
                    Repertory=model.Repertory
                };
                db.Product.Add(product);
                db.SaveChanges();
                model.Id = product.Id;
                if(product.Id>0)
                {
                    //save product image
                    foreach(BProductImage image in model.Images)
                    {
                        if(image.Image==null)
                        {
                            continue;
                        }
                        image.Product = new BProduct() { Id=product.Id };
                        MiOU.DAL.ProductImage file = new ProductImage()
                        {
                            ProductId = product.Id,
                            ImageId = image.Image.Id,
                            IsMain = image.IsMain
                        };
                        db.ProductImage.Add(file);
                    }

                    //save product price options
                    foreach(BProductPrice price in model.ProductPrices)
                    {
                        if (price.Category == null || price.Category.Id <= 0)
                        {
                            continue;
                        }
                        ProductPrice pPrice = new ProductPrice()
                        {
                            Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                            EvaluatedPriceId = 0,
                            Price = 0,
                            ProductCategoryId = price.Category.Id,
                            PriceCategory = price.Category.Id,
                            ProductId = product.Id,
                            UserId = CurrentLoginUser.User.UserId

                        };
                        db.ProductPrice.Add(pPrice);
                    }
                    db.SaveChanges();
                }
                return;
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }
            return;
        }

        /// <summary>
        /// Gets product price options
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<BProductPrice> GetProductPrices(int productId)
        {
            List<BProductPrice> prices = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from price in db.ProductPrice
                          join cate in db.PriceCategory on price.PriceCategory equals cate.Id into lcate
                          from llcate in lcate.DefaultIfEmpty()
                          join eprice in db.EvaluatedPrice on price.EvaluatedPriceId equals eprice.Id into leprice
                          from lleprice in leprice.DefaultIfEmpty()
                          where price.ProductId == productId
                          orderby price.Id ascending
                          select new BProductPrice
                          {
                              PriceCategory = new BPriceCategory() { Id = llcate.Id, Created = 0, Name = llcate.Name, Order = llcate.Order },
                              Price = price.Price,
                              EPrice = new BEvaluatedPrice() { Price = lleprice != null ? lleprice.Price : 0, Id= lleprice != null ? lleprice.Id : 0 }
                          };

                prices = tmp.ToList<BProductPrice>();
            }
            return prices;
        }

        /// <summary>
        /// Adds new product price options
        /// </summary>
        /// <param name="prices"></param>
        public void AddProductPrices(int productId,List<BProductPrice> prices)
        {
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                Product dbProduct = (from p in db.Product where p.Id == productId select p).FirstOrDefault<Product>();
                if (dbProduct == null)
                {
                    throw new MiOUException(string.Format("编号为{0}的产品不存在", productId));
                }
                if(!CurrentLoginUser.IsAdmin && CurrentLoginUser.User.UserId!=dbProduct.UserId)
                {
                    throw new MiOUException("只有管理员和藕主才可以添加藕品价格种类");
                }
                List<BProductPrice> existedPrices = GetProductPrices(productId);               
                foreach (BProductPrice p in prices)
                {
                    if (p.PriceCategory == null || p.PriceCategory.Id <= 0)
                    {
                        continue;
                    }

                    var ep = (from pp in existedPrices where pp.PriceCategory.Id == p.PriceCategory.Id select pp).FirstOrDefault<BProductPrice>();
                    if (ep != null)
                    {
                        continue;
                    }

                    ProductPrice dbPrice = new ProductPrice()
                    {
                        Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                        EvaluatedPriceId = 0,
                        PriceCategory = p.PriceCategory.Id,
                        ProductId = productId,
                        UserId = CurrentLoginUser.User.UserId
                    };

                    db.ProductPrice.Add(dbPrice);
                }
                db.SaveChanges();
                if (dbProduct.Status==1)
                {
                    GenerateProductPrice(dbProduct, db);
                }                
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Wechat public account home page
        /// </summary>
        /// <returns></returns>
        public List<BCategoryStatistic> GetTopCategoryStatistic()
        {
            List<BCategoryStatistic> list = new List<BCategoryStatistic>();
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                List<BCategory> categories = (from c in db.Category
                                              orderby c.Order
                                              select new BCategory
                                              {
                                                  Id=c.Id,
                                                  Name=c.Name,
                                                  Order=c.Order!=null ? (int)c.Order:0
                                              }).ToList<BCategory>();

                List<int> topIds = (from id in categories select id.Id).ToList<int>();
                List<Category> childCategories = (from c in db.Category where topIds.Contains(c.ParentId) select c).ToList<Category>();
                foreach(BCategory parentCategory in categories)
                {
                    BCategoryStatistic statistic = new BCategoryStatistic() { Category=parentCategory };
                    int[] cateIds = (from c in childCategories where c.ParentId==parentCategory.Id select c.Id).ToArray<int>();
                    if(cateIds!=null)
                    {
                        //latter will use ProductCount field of database Product object, this field will be plus 1 when a product is created.
                        List<Product> products = (from p in db.Product where cateIds.Contains(p.CategoryId) select p).ToList<Product>();
                        if(products!=null && products.Count>0)
                        {
                            statistic.ProductCount = products.Count;
                            statistic.SupplierCount = (from p in products select p.UserId).Distinct<int>().Count();
                            int[] productIds = (from p in products select p.Id).ToArray<int>();
                            statistic.FinishedOrderCount = (from o in db.Order where productIds.Contains(o.ProductId) select o.Id).Count();
                        }
                    }
                    list.Add(statistic);
                }

            }
            catch(MiOUException mex)
            {
                logger.Error(mex);
            }
            catch(Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                if(db!=null)
                {
                    db.Dispose();
                }
            }
            return list;
        }
    }
}
