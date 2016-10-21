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
        /// <param name="paymendId"></param>
        /// <param name="orderId"></param>
        /// <param name="orderNo"></param>
        /// <param name="paymentStatus"> </param>
        public void CompletePayment(int paymendId,int orderId,string orderNo, PaymentStatus paymentStatus)
        {
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                OrderPayment payment = (from op in db.OrderPayment where op.OrderId==orderId && op.OrderNo==orderNo && op.Id==paymendId select op).FirstOrDefault<OrderPayment>();
                if(payment!=null)
                {
                    payment.Status = (int)paymentStatus;
                    if(paymentStatus == PaymentStatus.SUCCEED)
                    {
                        payment.Payed = true;
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

        public void GeneratePaymentRecord(ref BOrder order)
        {
            MiOUEntities db = null;
            try
            {
                OrderPayment payment = new OrderPayment();
                payment.PayCategory = 1;//定金
                payment.PayType = order.PayType.Id;
                payment.OrderId = order.Id;
                payment.OrderNo = order.OrderNo;
                payment.Payed = false;
                payment.PayedTime = 0;
                payment.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                payment.Amount = 0;
                db.OrderPayment.Add(payment);
                db.SaveChanges();
                if(order.Payments==null)
                {
                    order.Payments = new List<BOrderPayment>();
                }
                if (payment.Id > 0)
                {
                    order.Payments.Add(new BOrderPayment() { });
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
