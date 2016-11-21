function BaseMiOU() {
    this.GetCities = function () {
    }
    this.GetProvinces = function () {
    }
    this.GetChildAreas = function (parentId) {
    }
    this.GetPaytypes = function () {
    }
    this.GetTranfserTypes = function () {
    }
    this.GetDeliverTypes = function () {
    }
    this.GetRentTypes = function () {
    }
    this.GetPayCategories = function () {

    }
    this.GetMaintenanceTypes = function () {
    }
    this.GetPriceCategories = function () {
    }
}

function ProductUtil() {
    this.GetTopCategoryStatistic = function (rentType, callback) {
        $.post
          (
            '/api/Product/GetTopCategoryStatistic',
            { rentType: rentType },
            function (res) {
                if (res != 'undefined' && typeof (res) == 'object' && callback != 'undefined' && typeof (callback) == 'function') {
                    callback(res);
                }
            }
        );
    }
}
ProductUtil.prototype = new BaseMiOU();