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
        public ProductManagement(int userId) : base(userId)
        {
        }

        public ProductManagement(BUser user) : base(user)
        {

        }

        public void DeleteProductImages(int productId,List<int> images)
        {
            using (MiOUEntities db = new MiOUEntities())
            {
                Product product = (from pdt in db.Product where pdt.Id == productId select pdt).FirstOrDefault<Product>();
                if (product == null)
                {
                    throw new MiOUException("产品信息不存在");
                }
                bool delete = false;
                if(product.UserId!=CurrentLoginUser.User.Id)
                {
                    if(!CurrentLoginUser.IsAdmin)
                    {
                        throw new MiOUException("只有管理员和产品所有者才能删除产品图片");
                    }
                    else
                    {
                        delete = true;
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
                }
            }
        }

        public bool AuditProduct(int productId, int status, string message)
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
                product.AuditUserId = CurrentLoginUser.User.Id;
                db.SaveChanges();
                ret = true;
            }
            return ret;
        }

        public void CreateProduct(BProduct model)
        {
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
                    UserId = CurrentLoginUser.User.Id,
                    Pledge = 0,
                    Price = 0,
                    Updated = 0,
                    XPlot = "",
                    YPlot = ""
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
