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

function CommonUtil(){
    this.GetAreaByParent = function (parentId, callback) {
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

function openModalDialog(url, data) {
    $.get(
            url,
            data,
            function (res, status) {
                if (res != null && res != "undefined") {
                    var modal = $('<div class="modal fade" id="editRuoteModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"></div>').appendTo($('body'));
                    var modalDiv = $('<div class="modal-dialog" role="document"></div>').appendTo($(modal));
                    var modalContent = $('<div class="modal-content"></div>').appendTo($(modalDiv));
                    var modalBody = $('<div class="modal-body"></div>').appendTo($(modalContent)).html(res);
                    $(modal).modal();
                }
            }
     );
}