using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiOU.DAL;
using MiOU.Entities.Beans;
using MiOU.Entities.Models;
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

        public List<BCategory> GetHomeProdustListByCategory(int province)
        {
            List<BCategory> categories = new List<BCategory>();
            categories = GetCategories(0, true);           
            if(categories.Count>0)
            {
                using (MiOUEntities db = new MiOUEntities())
                {
                    foreach (BCategory category in categories)
                    {
                        int total = 0;
                        List<BProduct> products = SearchProducts(null, new int[] { 1}, 0, 0, category.Id,0, 0,province,  0, 0,null, 10, 1, true, out total, ProductOrderField.RENTTIMES);
                        category.HotProducts = products;
                    }
                }                    
            }           
            return categories;
        }

        public BCategory GetHomeProdusByCategory(int province,int categoryid)
        {
            BCategory category = GetCategory(categoryid, true);
            if (category!=null)
            {
                using (MiOUEntities db = new MiOUEntities())
                {
                    int total = 0;
                    List<BProduct> products = null;
                    if(category.ParentId>0)
                    {
                        products = SearchProducts(null, new int[] { 1 }, 0, 0, 0, category.Id, 0, province, 0, 0, null, 10, 1, true, out total, ProductOrderField.RENTTIMES);
                    }else
                    {
                        products = SearchProducts(null, new int[] { 1 }, 0, 0,category.Id,0, 0, province, 0, 0, null, 10, 1, true, out total, ProductOrderField.RENTTIMES);
                    }                   
                    category.HotProducts = products;
                }
            }
            return category;
        }

        public ProductManagement(BUser user) : base(user)
        {

        }

        public bool UpdateProductLevel(BProductLevel level)
        {
            if(level.Id<=0)
            {
                throw new MiOUException("修改产品等级时，等级编号不能为0");
            }

            return SaveProductLevel(level);
        }

        public bool CreateNewProductLevel(BProductLevel level)
        {           
            if (string.IsNullOrEmpty(level.Name))
            {
                throw new MiOUException("产品等级名称不能为空");
            }
            if (string.IsNullOrEmpty(level.RentableVipLevels))
            {
                throw new MiOUException("产品等级对应的可借用VIP级别不能为空");
            }
            if(level.StartPrice<=0 && level.EndPrice<=0)
            {
                throw new MiOUException("产品等级价格区间的起始价格和终止价格不能同时为0");
            }
            if (level.StartPrice >0 && level.EndPrice <= 0)
            {
                throw new MiOUException("产品等级价格区间的起始价格不为0时的终止价格不能为0");
            }
            if(level.StartPrice>level.EndPrice)
            {
                throw new MiOUException("产品等级起始价格不能大于终止价格");
            }
            
            return SaveProductLevel(level);
        }

        public List<BEvaluatedPrice> GetProductLevelPrices(int productLevel)
        {
            List<BEvaluatedPrice> prices = null;
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from p in db.EvaluatedPrice
                          join pc in db.PriceCategory on p.PriceCategory equals pc.Id into lpc
                          from llpc in lpc.DefaultIfEmpty()
                          join epc in db.EvaluatedPriceCategory on p.EvaluatedPriceCategory equals epc.Id into lepc
                          from llepc in lepc.DefaultIfEmpty()
                          where p.EvaluatedPriceCategory == productLevel
                          orderby p.EvaluatedPriceCategory  orderby p.PriceCategory
                          select new BEvaluatedPrice
                          {
                              Catetegory = new BPriceCategory { Id= p.PriceCategory,Name= llpc.Name },
                              Created = p.Created,
                              Id = p.Id,
                              Price = p.Price,
                              ProductLevel = new BProductLevel { Id= p.EvaluatedPriceCategory,Name= llepc.Name },                            
                          };

                if(productLevel>0)
                {
                    tmp = tmp.Where(p=>p.ProductLevel.Id == productLevel);
                }

                prices = tmp.ToList<BEvaluatedPrice>();
            }
            return prices;
        }

        public bool SaveProductLevelPrices(int productLevelId, string[] epIds, string[] values)
        {
            bool result = false;           
            if(CurrentLoginUser==null)
            {
                throw new MiOUException("请先登录");
            }
            if(epIds==null || epIds.Length==0)
            {
                throw new MiOUException("参数错误");
            }
            if (values == null || values.Length == 0)
            {
                throw new MiOUException("参数错误");
            }
            if(epIds.Length!=values.Length)
            {
                throw new MiOUException("参数错误");
            }
            if (!CurrentLoginUser.IsAdmin)
            {
                throw new MiOUException("只有管理员账户才能执行此操作");
            }
            if(!CurrentLoginUser.Permission.UPDATE_PRODUCT_LEVEL_PRICES)
            {
                throw new MiOUException("没有更新产品等级租金的权限");
            }
            using (MiOUEntities db = new MiOUEntities())
            {
                EvaluatedPriceCategory epcate = (from ep in db.EvaluatedPriceCategory where ep.Id == productLevelId select ep).FirstOrDefault<EvaluatedPriceCategory>();
                if (epcate == null)
                {
                    throw new MiOUException("产品等级不存在");
                }
                int[] ids = (from s in epIds select int.Parse(s)).ToArray<int>();
                List<EvaluatedPrice> prices = (from pr in db.EvaluatedPrice where ids.Contains(pr.Id) select pr).ToList<EvaluatedPrice>();
                if(prices.Count==0)
                {
                    throw new MiOUException(string.Format("参数错误，没有找到任何租金数据 - 产品等级 {0}",epcate.Name!=null?epcate.Name:""));
                }
                if(prices.Count!=values.Length)
                {
                    throw new MiOUException(string.Format("参数错误，没有找到任何租金数据 - 产品等级 {0}", epcate.Name != null ? epcate.Name : ""));
                }
                
                for(int i=0;i<epIds.Length;i++)
                {
                    EvaluatedPrice tmp = (from p in prices where p.Id==int.Parse(epIds[i]) select p).FirstOrDefault<EvaluatedPrice>();
                    if(tmp==null)
                    {
                        logger.Warn(string.Format("Evaluated price record doesn't exist for No - {0} of Product Level {1}", epIds[i], epcate.Name != null ? epcate.Name : ""));
                        continue;
                    }
                    if(tmp.Price!=float.Parse(values[i]))
                    {
                        tmp.Price = float.Parse(values[i]);
                        tmp.Updated = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                        tmp.UpdatedUserId = CurrentLoginUser.User.UserId;
                    }
                }

                db.SaveChanges();
                result = true;
            }
            return result;
        }

        private bool SaveProductLevel(BProductLevel level)
        {
            if (!CurrentLoginUser.IsAdmin)
            {
                throw new MiOUException("没有权限创建或修改产品物资等级");
            }
            if (!CurrentLoginUser.Permission.UPDATE_PRODUCT_LEVEL)
            {
                throw new MiOUException("没有更新产品等级信息");
            }
            bool ret = false;
            bool newLevel = false;
            bool checkPrice = false;
            using (MiOUEntities db = new MiOUEntities())
            {
                List<BProductLevel> all = GetProductLevels(0,0,0,null);
                EvaluatedPriceCategory dbLevel = null;
                if (level.Id > 0)
                {
                    dbLevel = (from l in db.EvaluatedPriceCategory where l.Id == level.Id select l).FirstOrDefault<EvaluatedPriceCategory>();
                    if (dbLevel == null)
                    {
                        throw new MiOUException("产品等级不存在");
                    }
                    if(!string.IsNullOrEmpty(level.Name))
                    dbLevel.Name = level.Name;

                    if (level.StartPrice != dbLevel.StartPrice)
                    {
                        checkPrice = true;
                        dbLevel.StartPrice = level.StartPrice;
                    }


                    if (dbLevel.EndPrice != level.EndPrice)
                    {
                        checkPrice = true;
                        dbLevel.EndPrice = level.EndPrice;
                    }
                   
                    dbLevel.Updated = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                    dbLevel.UpdatedBy = CurrentLoginUser.User.UserId;
                    dbLevel.VIPRentLevel = level.RentableVipLevels;
                }
                else
                {
                    newLevel = true;
                    checkPrice = true;
                    dbLevel = new EvaluatedPriceCategory()
                    {
                        Name = level.Name,
                        Description = level.Description,
                        Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                        CreatedBy = (level.CreatedBy != null && level.CreatedBy.User != null) ? level.CreatedBy.User.UserId : CurrentLoginUser.User.UserId,
                        Updated = 0,
                        UpdatedBy = 0,
                        VIPRentLevel = level.RentableVipLevels,
                        StartPrice = level.StartPrice,
                        EndPrice = level.EndPrice
                    };
                }

                if (checkPrice)
                {
                    List<BProductLevel> tmpLevels = (from t in all where (t.StartPrice == dbLevel.StartPrice && t.EndPrice == dbLevel.EndPrice) || t.Name.Contains(dbLevel.Name) select t).ToList<BProductLevel>();
                    if (tmpLevels.Count > 0)
                    {
                        throw new MiOUException(string.Format("价个区间为{0} - {1}的等级已经存在", dbLevel.StartPrice, dbLevel.EndPrice));
                    }
                    tmpLevels = (from t in all where ((t.StartPrice<=dbLevel.StartPrice && t.EndPrice>=dbLevel.StartPrice) || (t.StartPrice<=dbLevel.EndPrice && t.EndPrice>=dbLevel.EndPrice))select t).ToList<BProductLevel>();
                    if (tmpLevels.Count > 0)
                    {
                        throw new MiOUException(string.Format("价个区间为{0} - {1}的价格区间不唯一，与其他等级有重叠", dbLevel.StartPrice, dbLevel.EndPrice));
                    }
                }
               
                if(newLevel)
                {
                    db.EvaluatedPriceCategory.Add(dbLevel);
                }

                db.SaveChanges();
                //Generate prices by price categories
                List<EvaluatedPrice> prices = (from p in db.EvaluatedPrice where p.EvaluatedPriceCategory== dbLevel.Id select p).ToList<EvaluatedPrice>();
                List<PriceCategory> categories = (from c in db.PriceCategory orderby c.Id select c).ToList<PriceCategory>();
                foreach(PriceCategory cate in categories)
                {
                    EvaluatedPrice tmpprice = (from p in prices where p.PriceCategory == cate.Id select p).FirstOrDefault<EvaluatedPrice>();
                    if(tmpprice==null)
                    {
                        tmpprice = new EvaluatedPrice()
                        {
                            Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                            EvaluatedPriceCategory = dbLevel.Id,
                            Price = 0,
                            PriceCategory = cate.Id,
                            Updated = 0,
                            UpdatedUserId = 0,
                            UserId = CurrentLoginUser.User.UserId
                        };
                        db.EvaluatedPrice.Add(tmpprice);
                    }
                }
                db.SaveChanges();
                ret = true;
            }
            return ret;
        }

        public List<BProductLevel> GetProductLevels(int id,float start,float end,string vipstrs)
        {
            List<BProductLevel> levels = new List<BProductLevel>();
            using (MiOUEntities db = new MiOUEntities())
            {
                var tmp = from l in db.EvaluatedPriceCategory
                          join cu in db.User on l.CreatedBy equals cu.UserId into lcu
                          from llcu in lcu.DefaultIfEmpty()
                          join uu in db.User on l.UpdatedBy equals uu.UserId into luu
                          from lluu in luu.DefaultIfEmpty()
                          orderby l.StartPrice ascending
                          select new BProductLevel
                          {
                              Id=l.Id,
                              StartPrice = l.StartPrice,
                              EndPrice = l.EndPrice,
                              Created = l.Created,
                              Updated = l.Updated,
                              CreatedBy = llcu != null ? new BUser { User = llcu } : null,
                              UpdatedBy = lluu != null ? new BUser { User = lluu } : null,
                              Name = l.Name,
                              Description = l.Description,
                              RentableVipLevels = l.VIPRentLevel
                          };

                if(id>0)
                {
                    tmp = tmp.Where(t=>t.Id == id);
                }
                if(start>0)
                {
                    tmp = tmp.Where(t => t.StartPrice>= start);
                }
                if (end > 0)
                {
                    tmp = tmp.Where(t => t.EndPrice >= end);
                }
                if(!string.IsNullOrEmpty(vipstrs))
                {
                    tmp = tmp.Where(t => t.RentableVipLevels.Contains(vipstrs));
                }
                List<BVIPLevel> vips = GetVipLevels();
                levels = tmp.ToList<BProductLevel>();
                foreach(BProductLevel plevel in levels)
                {
                    string rentVips = plevel.RentableVipLevels;
                    if(!string.IsNullOrEmpty(rentVips))
                    {
                        string[] vipsAarry = rentVips.Split(',');
                        List<BVIPLevel> tmpVips = (from v in vips where vipsAarry.Contains(v.Id.ToString()) select v).ToList<BVIPLevel>();
                        plevel.RentableVips = tmpVips;
                    }
                    
                }
            }
            return levels;
        }

        public BProduct GetProductDetail(int productId)
        {
            BProduct product = null;
            if(productId<=0)
            {
                throw new MiOUException("产品信息不存在");
            }
            int total = 0;
            List<BProduct> products = SearchProducts(new int[] { }, null, 0, 0, 0, 0, 0, 0, 0, 0, null, 10, 1, true, out total);
            if (total != 1)
            {
                throw new MiOUException("产品信息不存在");
            }
            product = products[0];
            return product;
        }
        public List<BProduct> SearchProducts(int[] productIds,int[] states,int userId,int auditUserId, int pId,int cId,int rentType,int provinceId,int cityId,int districtId,string keyword,int pageSize,int page,bool getDetail,out int total, ProductOrderField pOrder= ProductOrderField.RENTTIMES,int manageType=1)
        {
            List<BProduct> products = null;
            MiOUEntities db = null;
            total = 0;
            try
            {
                db = new MiOUEntities();
                var linq = from p in db.Product
                           join cate in db.Category on p.CategoryId equals cate.Id into lcate
                           from llcate in lcate.DefaultIfEmpty()
                           join province in db.Area on p.Province equals province.Id into lprovince
                           from llprovince in lprovince.DefaultIfEmpty()
                           join city in db.Area on p.City equals city.Id into lcity
                           from llcity in lcity.DefaultIfEmpty()
                           join district in db.Area on p.District equals district.Id into ldistrict
                           from lldistrict in ldistrict.DefaultIfEmpty()
                           join shipping in db.DeliveryType on p.DeliveryType equals shipping.Id into lshipping
                           from llshipping in lshipping.DefaultIfEmpty()
                           join vip in db.VipLevel on p.VIPLevel equals vip.Id into lvip
                           from llvip in lvip.DefaultIfEmpty()
                           join rtype in db.RentType on p.RentType equals rtype.Id into lrtype
                           from llrtype in lrtype.DefaultIfEmpty()
                           join owner in db.User on p.UserId equals owner.UserId into lowner
                           from llowner in lowner.DefaultIfEmpty()                         
                           join auser in db.User on p.AuditUserId equals auser.UserId into lauser
                           from llauser in lauser.DefaultIfEmpty()
                           join level in db.EvaluatedPriceCategory on p.EvaluatedPriceCategoryId equals level.Id into llevel
                           from lllevel in llevel.DefaultIfEmpty()
                           select new BProduct
                           {
                               Id = p.Id,
                               Name = p.Name,                             
                               Category = new BCategory { Id = p.CategoryId, Name = llcate.Name, ParentId=llcate.ParentId },
                               District = new BArea { Id = p.District, Name = lldistrict.Name },
                               Province = new BArea { Id = p.Province, Name = llprovince.Name },
                               City = new BArea { Id = p.City, Name = llcity.Name },
                               Address = p.Address,
                               Nearby = p.NearBy,
                               Apartment = p.Apartment,
                               Contact = p.Contact,
                               Phone = p.Phone,
                               Price = p.Price,
                               Percentage = p.Percentage,
                               DeliveryType = new BObject { Id = p.DeliveryType, Name = llshipping.Name },
                               VIPRentLevel = new BVIPLevel { Id = p.VIPLevel, Name = llvip.Name },
                               RentType = new BObject { Id = p.RentType, Name = llrtype.Name },
                               AuditStatus = (ProductStatus)p.Status,
                               Status = (RentStatus)p.Status,
                               Created = p.Created,
                               Updated = p.Updated,
                               AuditTime = p.AuditTime,
                               AuditMessage = p.AuditMessage,
                               AuditUser = new BUser { User= llauser },
                               Description = p.Description,
                               Repertory = p.Repertory,
                               RentOutQuantity= p.RentOutQuantity,
                               User = llowner != null ? new BUser() { User = llowner } : null,
                               XPlot = p.XPlot,
                               YPlot = p.YPlot,
                               RentTimes= p.RentTimes,
                               ManageType=p.ManageType,
                               ProductLevel= lllevel!=null?new BProductLevel { Name= lllevel.Name,Id= lllevel.Id,StartPrice= lllevel.StartPrice,EndPrice= lllevel.EndPrice } :null
                           };
                if(productIds!=null && productIds.Length>0)
                {
                    linq = linq.Where(o=>productIds.Contains(o.Id));
                }
                if(userId>0)
                {
                    linq = linq.Where(a=>a.User.User.UserId==userId);
                }
                if (auditUserId > 0)
                {
                    linq = linq.Where(a => a.AuditUser.User.UserId == auditUserId);
                }
                if (states!=null && states.Length>0)
                {
                    linq = linq.Where(a =>states.Contains((int)a.AuditStatus));
                }
                if(rentType>0)
                {
                    linq = linq.Where(a=>a.RentType.Id==rentType);
                }
                if(pId>0)
                {
                    linq = linq.Where(a=>(from c in db.Category where c.ParentId==pId select c.Id).Contains(a.Category.Id));
                }
                if(cId>0)
                {
                    linq = linq.Where(a=>a.Category.Id==cId);
                }
                if (provinceId > 0)
                {
                    linq = linq.Where(a => a.Province.Id == provinceId);
                }
                if (cityId > 0)
                {
                    linq = linq.Where(a => a.City.Id == cityId);
                }
                if (districtId > 0)
                {
                    linq = linq.Where(a => a.District.Id == districtId);
                }
                if(!string.IsNullOrEmpty(keyword))
                {
                    linq = linq.Where(a=>(a.Name.Contains(keyword) || a.Address.Contains(keyword) || a.Nearby.Contains(keyword) || a.Apartment.Contains(keyword)));
                }
                if(manageType>0)
                {
                    linq = linq.Where(a=>a.ManageType==manageType);
                }
                switch(pOrder)
                {
                    case ProductOrderField.OWNER:
                        linq = linq.OrderByDescending(a => a.User.Id);
                        break;
                    case ProductOrderField.RENTTIMES:
                        linq = linq.OrderByDescending(a => a.RentTimes);
                        break;
                    case ProductOrderField.RENTTYPE:
                        linq = linq.OrderByDescending(a => a.RentType.Id);
                        break;
                    case ProductOrderField.SHIPTYPE:
                        linq = linq.OrderByDescending(a => a.DeliveryType.Id);
                        break;
                    case ProductOrderField.UPDATED:
                        linq = linq.OrderByDescending(a => a.Updated);
                        break;
                    case ProductOrderField.REPERTORY:
                        linq = linq.OrderByDescending(a => a.Repertory);
                        break;
                    case ProductOrderField.CREATED:
                    default:
                        linq = linq.OrderByDescending(a => a.Created);
                        break;
                }
                
                total = linq.Count();
                if(page<=0)
                {
                    page = 1;
                }
                products = linq.Skip((page - 1) * pageSize).Take(pageSize).ToList<BProduct>();
                List<Category> parentCategories = (from c in db.Category where c.ParentId == 0 select c).ToList<Category>();
                foreach(BProduct p in products)
                {
                    p.PCategory = (from pc in parentCategories where pc.Id == p.Category.ParentId select new BCategory() { Id= pc.Id,Name=pc.Name }).FirstOrDefault<BCategory>();
                }

                if(getDetail)
                {
                    int[] ids = (from p in products select p.Id).ToArray<int>();
                    List<BProductPrice> prices;                        
                    var tmpPrices=from price in db.ProductPrice where ids.Contains(price.ProductId) 
                                  join pcate in db.PriceCategory on price.PriceCategory equals pcate.Id into lpcate
                                  from llcate in lpcate.DefaultIfEmpty()
                                  join eprice in db.EvaluatedPrice on price.EvaluatedPriceId equals eprice.Id into leprice
                                  from lleprice in leprice.DefaultIfEmpty()
                                  select new BProductPrice
                                  {
                                     Price= price.Price,
                                     Product= new BProduct { Id= price.ProductId },
                                     PriceCategory= llcate!=null? new BPriceCategory { Id= llcate.Id, Name= llcate.Name }:null,
                                     EPrice= lleprice!=null? new BEvaluatedPrice { Price= lleprice.Price }:null,
                                     Enabled= price.Enabled
                                  };

                    prices = tmpPrices.ToList<BProductPrice>();

                    List<BProductImage> images;
                    var tmpImages = from image in db.ProductImage
                                    join file in db.File on image.ImageId equals file.Id into lfile
                                    from llfile in lfile.DefaultIfEmpty()
                                    where ids.Contains(image.ProductId)
                                    orderby image.IsMain
                                    select new BProductImage
                                    {
                                        Id = image.Id,
                                        Image = llfile != null ? new BFile { Created = llfile.Created, Id = llfile.Id, Name = llfile.Name, UserId = llfile.UserId, Path = llfile.Path, Ext = llfile.Ext } : null,
                                        IsMain = image.IsMain,
                                        Product = new BProduct { Id = image.ProductId }
                                    };
                    images = tmpImages.ToList<BProductImage>();


                    foreach (BProduct p in products)
                    {
                        p.ProductPrices = (from price in prices where price.Product.Id== p.Id select price).ToList<BProductPrice>();
                        p.Images = (from image in images where image.Product.Id == p.Id select image).ToList<BProductImage>();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }

            return products;
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

        public bool AuditProduct(MAuditProduct auditProduct)
        {
            if (auditProduct.AuditResult == 2)
            {
                auditProduct.EvalutedPercentage = 0;
                auditProduct.EvalutedPrice = 0;
            }
            else
            {
                auditProduct.EvalutedPercentage = auditProduct.EvalutedPercentage / 100;
            }
            return AuditProduct(auditProduct.ProductId,auditProduct.ProductLevel, auditProduct.AuditResult, auditProduct.Message, auditProduct.EvalutedPrice, auditProduct.EvalutedPercentage);
        }

        public bool AuditProduct(int productId,int productLevel, int status, string message,float ePrice,float percentage)
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
                    return true;
                }
                if(product.EvaluatedPriceCategoryId>0)
                {
                    throw new MiOUException("藕品等级已经设置过，不能重复审核藕品");
                }

                product.Status = status;
                product.AuditMessage = message;
                product.AuditTime = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
                product.AuditUserId = CurrentLoginUser.User.UserId;
                product.EvaluatedPercentage = percentage;
                product.EvaluatedPrice = ePrice;
                product.EvaluatedPriceCategoryId = productLevel;
                db.SaveChanges();
                GenerateProductPrice(product, db);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// 租赁出去 count为负整数，归还count为正整数
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
            List<EvaluatedPrice> eprices = (from eprice in db.EvaluatedPrice where eprice.EvaluatedPriceCategory==product.EvaluatedPriceCategoryId && priceCategories.Contains(eprice.PriceCategory) select eprice).ToList<EvaluatedPrice>();
            foreach(EvaluatedPrice eprice in eprices)
            {
                ProductPrice price = (from pdtPrice in prices where pdtPrice.ProductId == product.Id && pdtPrice.PriceCategory == eprice.PriceCategory select pdtPrice).FirstOrDefault<ProductPrice>();
                if (price != null)
                {
                    price.EvaluatedPriceId = eprice.Id;
                    //price.Price = eprice.Price;                  
                }
            }
            db.SaveChanges();
        }

        public void UpdateProduct(MProduct product)
        {
            if(product==null || product.Id<=0)
            {
                throw new MiOUException("藕品数据不存在");
            }
            int total = 0;
            List<BProduct> products = SearchProducts(new int[] { product.Id }, null, 0, 0, 0, 0, 0, 0, 0, 0, null, 1, 1, true, out total);
            if(products.Count==0)
            {
                throw new MiOUException(string.Format("Id为{0}的藕品数据不存在"));
            }
            if(products.Count>1)
            {
                throw new MiOUException(string.Format("Id为{0}的藕品数据不唯一"));
            }

            BProduct existedProduct = products[0];
            if(existedProduct.User.User.UserId!=CurrentLoginUser.User.UserId)
            {
                throw new MiOUException("请不要尝试修改别人的藕品");
            }

            if(string.IsNullOrEmpty(product.Name))
            {
                throw new MiOUException("藕品名称不能为空");
            }
            if (product.Repertory<1)
            {
                throw new MiOUException("藕品库存必须大于或等于1");
            }
            if(product.Price<=0)
            {
                throw new MiOUException("藕品价格必须大于零");
            }
            if(product.Price!=existedProduct.Price)
            {
                throw new MiOUException("藕品价格不能修改");
            }
            if(string.IsNullOrEmpty(product.PriceCotegories))
            {
                throw new MiOUException("租赁形式必须选择一个，如日租，周租...");
            }
            
            List<ProductImage> newImages = new List<ProductImage>();
            if(!string.IsNullOrEmpty(product.PhotoIds))
            {
                string[] photoes = product.PhotoIds.Split(',');
                foreach(string photo in photoes)
                {
                    int photoId = 0;
                    int.TryParse(photo,out photoId);
                    if(photoId>0)
                    {
                        newImages.Add(new ProductImage {ImageId=photoId,ProductId= product.Id });
                    }
                }
            }
            //get removed images
            List<int> removedImages = new List<int>();
            List<int> newUploadedImages = new List<int>();
            foreach (BProductImage image in existedProduct.Images)
            {
                ProductImage file = (from f in newImages where f.ImageId== image.Image.Id select f).FirstOrDefault<ProductImage>();
                if(file==null)
                {
                    //The image has been removed from frontend
                    removedImages.Add(image.Image.Id);
                }
            }
            foreach(ProductImage image in newImages)
            {
                BProductImage pImage = (from pi in existedProduct.Images where pi.Image.Id==image.ImageId select pi).FirstOrDefault<BProductImage>();
                if(pImage==null)
                {
                    newUploadedImages.Add(image.ImageId);
                }
            }
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();                
                lock (obj)
                {
                    Product dbProduct = (from p in db.Product where p.Id == product.Id select p).FirstOrDefault<Product>();
                    dbProduct.Repertory = product.Repertory;
                    dbProduct.Name = product.Name;
                    dbProduct.Contact = product.Contact;
                    dbProduct.Phone = product.Phone;
                    dbProduct.Address = product.Address;
                    dbProduct.DeliveryType = product.DeliveryType;
                    dbProduct.RentType = product.RentType;
                    dbProduct.Description = product.Description;
                    db.SaveChanges();
                }

                List<ProductPrice> newAdded = new List<ProductPrice>();
                List<int> removedPcates = new List<int>();                    
                string[] priceCate = product.PriceCotegories.Split(',');
                //handle new added rent price category
                foreach(string pcate in priceCate)
                {
                    int pcateId = 0;
                    int.TryParse(pcate, out pcateId);
                    if(pcateId>0)
                    {
                        BProductPrice price = (from p in existedProduct.ProductPrices where p.PriceCategory.Id == pcateId select p).FirstOrDefault<BProductPrice>();
                        if(price==null)
                        {
                            EvaluatedPrice ePrice = (from ep in db.EvaluatedPrice where ep.EvaluatedPriceCategory == existedProduct.ProductLevel.Id && ep.PriceCategory == pcateId select ep).FirstOrDefault<EvaluatedPrice>();
                            int ePriceId = ePrice!=null?ePrice.Id:0;
                            ProductPrice tmpPrice = new ProductPrice { Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now), EvaluatedPriceId = ePriceId, PriceCategory = pcateId, ProductCategoryId = product.ChildCategoryId, ProductId = product.Id, UserId = CurrentLoginUser.User.UserId };
                            newAdded.Add(tmpPrice);
                            db.ProductPrice.Add(tmpPrice);
                        }
                        else
                        {
                            ProductPrice pprice = (from p in db.ProductPrice where p.ProductId==product.Id && p.PriceCategory==pcateId select p).FirstOrDefault<ProductPrice>();
                            if(pprice!=null)
                            {
                                pprice.Enabled = true;
                            }
                        }
                    }                    
                }

                //handle removed rent price category
                List<ProductPrice> productPrices = (from pp in db.ProductPrice where pp.ProductId== product.Id select pp).ToList<ProductPrice>();
                foreach(ProductPrice price in productPrices)
                {
                    string tmpPrice = (from p in priceCate where price.PriceCategory.ToString()==p select p).FirstOrDefault<string>();
                    if(string.IsNullOrEmpty(tmpPrice))
                    {
                        //the existed product price category has been removed from frontend
                        price.Enabled = false;
                    }
                }

                //handle new uploaded images
                foreach(int imageId in newUploadedImages)
                {
                    ProductImage nPImage = new ProductImage() { ImageId=imageId, IsMain=false, ProductId=product.Id };
                    db.ProductImage.Add(nPImage);
                }

                //handle removed images
                //List<File> images = (from f in db.File where removedImages.Contains(f.Id) select f).ToList<File>();
                List<ProductImage> pImages = (from pi in db.ProductImage where pi.ProductId==product.Id && removedImages.Contains(pi.ImageId) select pi).ToList<ProductImage>();
                if(pImages.Count>0)
                {
                    UploadFileManagement fileMgr = new UploadFileManagement(CurrentLoginUser);
                    foreach (ProductImage file in pImages)
                    {                        
                        if(fileMgr.RemoveFile(file.ImageId))
                        {
                            db.ProductImage.Remove(file);
                        }
                    }
                }
                db.SaveChanges();
               
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

        public void CreateProduct(MProduct product)
        {
            if (product == null)
            {
                throw new MiOUException("藕品数据不正确");
            }
            
            BProduct model = new BProduct();
            model.Name = product.Name;
            model.DeliveryType = new BObject() { Id= product.DeliveryType };
            model.RentType = new BObject() { Id = product.RentType };
            model.Repertory = product.Repertory;
            model.Description = product.Description;
            model.Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
            model.Percentage = product.Percentage;
            model.Price = product.Price;
            model.Category = new BCategory() { Id = product.ChildCategoryId };
            model.PCategory = new BCategory() { Id = product.CategoryId };
            model.ManageType = product.ManageType;
            if(!string.IsNullOrEmpty(product.PhotoIds))
            {
                string[] files = product.PhotoIds.Split(',');
                if(files.Length>0)
                {
                    int count = 0;
                    bool isMain = false;
                    foreach(string file in files)
                    {
                        if(count==0)
                        {
                            isMain = true;
                        }else
                        {
                            isMain = false;
                        }
                        count++;
                        if(model.Images==null)
                        {
                            model.Images = new List<BProductImage>();
                        }
                        int fileId = 0;
                        int.TryParse(file,out fileId);
                        if(fileId<=0)
                        {
                            continue;
                        }
                        model.Images.Add(new BProductImage() { Image=new BFile() { Id=fileId }, IsMain=isMain });
                    }
                }
            }

            if(!string.IsNullOrEmpty(product.PriceCotegories))
            {
                string[] pcs = product.PriceCotegories.Split(',');
                if(pcs.Length>0)
                {
                    foreach(string cate in pcs)
                    {
                        if(model.ProductPrices==null)
                        {
                            model.ProductPrices = new List<BProductPrice>();
                        }
                        if(string.IsNullOrEmpty(cate))
                        {
                            continue;
                        }
                        int cateId = 0;
                        int.TryParse(cate, out cateId);
                        if (cateId <= 0)
                        {
                            continue;
                        }
                        model.ProductPrices.Add(new BProductPrice() { Category = new BCategory() { Id= product.ChildCategoryId }, CreatedBy = CurrentLoginUser, PriceCategory = new BPriceCategory() { Id= cateId } } );
                    }
                }
            }
            model.Province = new BArea() { Id= CurrentLoginUser.User.Province };
            model.City = new BArea() { Id = CurrentLoginUser.User.City };
            model.District = new BArea() { Id = CurrentLoginUser.User.District };
            model.Address = !string.IsNullOrEmpty(product.Address)?product.Address: CurrentLoginUser.User.Address;
            model.Contact = !string.IsNullOrEmpty(product.Contact) ? product.Contact : CurrentLoginUser.User.Name;
            model.Phone = !string.IsNullOrEmpty(product.Phone) ? product.Phone : CurrentLoginUser.User.Phone;
            CreateProduct(model);
        }

        public void CreateProduct(BProduct model)
        {
            if(model==null)
            {
                throw new MiOUException("藕品数据不正确");
            }
            if(model.Category==null || model.Category.Id<=0)
            {
                throw new MiOUException("藕品类别不能为空");
            }
            if(string.IsNullOrEmpty(model.Name))
            {
                throw new MiOUException("藕品名称不能为空");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                throw new MiOUException("藕品描述不能为空");
            }
            //if(string.IsNullOrEmpty(model.Address))
            //{
            //    throw new MiOUException("藕品地址不能为空");
            //}
            if (model.DeliveryType==null || model.DeliveryType.Id<=0)
            {
                throw new MiOUException("藕品交付方式不能为空");
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
            //if (model.District== null || model.District.Id <= 0)
            //{
            //    throw new MiOUException("区县不能为空");
            //}
            if(model.Percentage==0)
            {
                throw new MiOUException("藕品成色不能为空");
            }
            if(model.Images==null)
            {
                throw new MiOUException("缺少藕品图片");
            }
            if (model.Images.Count<2)
            {
                throw new MiOUException("藕品图片至少两张");
            }
            if(model.ProductPrices==null || model.ProductPrices.Count<=0)
            {
                throw new MiOUException("藕品的租赁方式至少勾选一种如（日租，周租，月租...）");
            }
            if(CurrentLoginUser.User.UserType==1 && model.Repertory>1)
            {
                throw new MiOUException("个人用户的藕品库存必须等于1，如有同种藕品请新添加一个藕品");
            }
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                Product product = new Product()
                {
                    Name=model.Name,
                    Address = model.Address!=null?model.Address:"",
                    AuditMessage = "",
                    AuditTime = 0,
                    AuditUserId = 0,
                    CategoryId = model.Category.Id,
                    Created = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now),
                    City = model.City!=null? model.City.Id:0,
                    DeliveryType = model.DeliveryType.Id,
                    Description = model.Description,
                    District = model.District!=null? model.District.Id:0,
                    Percentage = model.Percentage,                   
                    Province = model.Province!=null? model.Province.Id:0,
                    RentType = model.RentType.Id,
                    Status = 0,
                    UserId = CurrentLoginUser.User!=null? CurrentLoginUser.User.UserId:0,
                    Pledge = 0,
                    Price = model.Price,
                    Updated = 0,
                    XPlot = "",
                    YPlot = "",
                    Repertory=model.Repertory,
                    Apartment=model.Apartment,
                    NearBy=model.Nearby,
                    RentOutQuantity=0,
                    VIPLevel= model.VIPRentLevel!=null?model.VIPRentLevel.Id:0,
                    Contact= model.Contact!=null?model.Contact:"",
                    Phone= model.Phone!=null?model.Phone:"",
                    ManageType= model.ManageType
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
                            PriceCategory = price.PriceCategory.Id,
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
        public List<BCategoryStatistic> GetTopCategoryStatistic(int rentType)
        {
            List<BCategoryStatistic> list = new List<BCategoryStatistic>();
            MiOUEntities db = null;
            try
            {
                db = new MiOUEntities();
                List<BCategory> categories = (from c in db.Category
                                              where c.ParentId==0
                                              orderby c.Order
                                              select new BCategory
                                              {
                                                  Id=c.Id,
                                                  Name=c.Name,
                                                  IconPhotoMobile=c.PhotoMobile,
                                                  IconPhotoPC=c.PhotoPC,
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
                        parentCategory.ChildRen = (from c in childCategories where c.ParentId == parentCategory.Id select new BCategory { Id=c.Id,Name=c.Name }).ToList<BCategory>();
                        //latter will use ProductCount field of database Product object, this field will be plus 1 when a product is created.
                        List<Product> products = (from p in db.Product where cateIds.Contains(p.CategoryId) && p.RentType==rentType select p).ToList<Product>();
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
