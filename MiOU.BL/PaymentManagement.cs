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
   
    public class PaymentManagement:BaseManager
    {
        public PaymentManagement(int userId) : base(userId)
        {
        }

        public PaymentManagement(BUser user) : base(user)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="orderNo"></param>
        /// <param name="orderId"></param>
        /// <param name="paymentCategory"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        public List<BPayment> FindPayments(int paymentId, string orderNo, int orderId, int paymentCategory, int paymentStatus)
        {
            List<BPayment> payments = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from p in db.Payment
                          join o in db.Order on p.OrderId equals o.Id into lo
                          from llo in lo.DefaultIfEmpty()
                          join pcate in db.PayCategory on p.PayCategory equals pcate.Id into lpcate
                          from llpcate in lpcate.DefaultIfEmpty()
                          join ptype in db.PayType on p.PayType equals ptype.Id into lptype
                          from llptype in lptype.DefaultIfEmpty()
                          select new BPayment
                          {
                              Id = p.Id,
                              Created = p.Created,
                              Amount = p.Amount,
                              PayCategory = new BPayCategory { Id = p.PayCategory, Name = llpcate.Name },
                              Payed = p.Payed,
                              PayedTime = p.PayedTime,
                              PayType = new BPayType { Id = p.PayType, Name = llptype.Name },
                              Status = (PaymentStatus)p.Status,
                              Updated = p.UpdatedTime,
                              Order = llo != null ? new BOrder
                              {
                                  Id = p.OrderId,
                                  OrderNo = p.OrderNo,
                                  ContactName = llo.ContactName,
                                  ContactPhone = llo.ContactPhone,
                                  Created = llo.Created,
                                  Description = llo.Description,

                              } : null
                          };
                if (paymentId > 0)
                {
                    tmp = tmp.Where(a => a.Id == paymentId);
                }
                if (orderId > 0)
                {
                    tmp = tmp.Where(a => a.Order.Id == orderId);
                }
                if (string.IsNullOrEmpty(orderNo))
                {
                    tmp = tmp.Where(a => a.Order.OrderNo == orderNo);
                }
                if (paymentCategory > 0)
                {
                    tmp = tmp.Where(a => a.PayCategory.Id == paymentCategory);
                }
                if (paymentStatus > 0)
                {
                    tmp = tmp.Where(a => a.Status == (PaymentStatus)paymentStatus);
                }
                payments = tmp.ToList<BPayment>();
            }
            return payments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymendId"></param>
        /// <param name="orderId"></param>
        /// <param name="orderNo"></param>
        /// <param name="paymentStatus"> </param>
        public void UpdatePaymentStatus(int paymendId,PaymentStatus paymentStatus)
        {
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                Payment payment = null;
                if (paymendId > 0)
                {
                    payment = (from op in db.Payment where op.Id == paymendId select op).FirstOrDefault<Payment>();
                }
                else
                {
                    throw new MiOUException("请输入支付编号");
                }          
                
                if(payment!=null)
                {
                    payment.Status = (int)paymentStatus;
                    payment.UpdatedTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                    //update order status
                    Order dbOrder = null;
                    if (payment.OrderId > 0 && !string.IsNullOrEmpty(payment.OrderNo))
                    {
                        dbOrder = (from o in db.Order where o.OrderNo == payment.OrderNo && o.Id == payment.OrderId select o).FirstOrDefault<Order>();

                    }

                    if (paymentStatus == PaymentStatus.SUCCEED)
                    {
                        payment.Payed = true;
                        payment.PayedTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                        if (dbOrder != null)
                        {
                            switch (payment.PayCategory)
                            {
                                case 1:
                                    dbOrder.Status = 1;
                                    break;
                                case 2:
                                    dbOrder.Status = 2;
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    break;
                                case 5:
                                    break;
                                case 6:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else if(paymentStatus== PaymentStatus.CANCELED)
                    {
                        payment.PayedTime = 0;
                        
                        if (dbOrder != null)
                        {
                            switch (payment.PayCategory)
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                    dbOrder.Status = 9;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else if(paymentStatus == PaymentStatus.FAILED)
                    {
                        //
                    }
                    
                    db.SaveChanges();
                }
            }
            catch (MiOUException mex)
            {
                logger.Error(mex);
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

        public void GeneratePaymentRecord(ref BOrder order,int paymentCategory)
        {
            MiOUEntities db = null;
            try
            {
                Payment payment = new Payment();
                payment.PayCategory = paymentCategory;
                payment.PayType = order.PayType.Id;
                payment.OrderId = order.Id;
                payment.OrderNo = order.OrderNo;
                payment.Payed = false;
                payment.PayedTime = 0;
                payment.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                payment.Amount = 0;
                db.Payment.Add(payment);
                db.SaveChanges();
                if(order.Payments==null)
                {
                    order.Payments = new List<BPayment>();
                }
                if (payment.Id > 0)
                {
                    order.Payments.Add(new BPayment() { });
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
        }
    }
}
