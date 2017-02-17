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

function UserUtil() {
    this.EditAddress = function (id, callback) {
        $.post
         (
           '/api/User/EditAddress',
           { addressId: id },
           function (res) {
               if (res != 'undefined' && typeof (res) == 'object' && callback != 'undefined' && typeof (callback) == 'function') {
                   callback(res);
               }
           }
       );
    }
    this.DeleteAddress = function (id, callback) {
        
        $.post
         (
           '/api/User/DeleteAddress',
           { addressId: id },
           function (res) {
               if (res != 'undefined' && typeof (res) == 'object' && callback != 'undefined' && typeof (callback) == 'function') {
                   callback(res);
               }
           }
       );
    }
    this.DeleteAvactor = function (id, callback) {
       
        $.post
         (
           '/api/User/DeleteAvactor',
           { avactorId: id },
           function (res) {
               if (res != 'undefined' && typeof (res) == 'object' && callback != 'undefined' && typeof (callback) == 'function') {
                   callback(res);
               }
           }
       );
    }
    this.SetAvactor = function (id, callback) {
        $.post
         (
           '/api/User/SetAvactor',
           { avactorId: id },
           function (res) {
               if (res != 'undefined' && typeof (res) == 'object' && callback != 'undefined' && typeof (callback) == 'function') {
                   callback(res);
               }
           }
       );
    }
    this.SetDefaultAddress = function (id, callback) {

        $.post
         (
           '/api/User/SetDefaultAddress',
           { addressId: id },
           function (res) {
               if (res != 'undefined' && typeof (res) == 'object' && callback != 'undefined' && typeof (callback) == 'function') {
                   callback(res);
               }
           }
       );
    }

    this.CreateAddress = function (callback) {
        var _this = this;
        var url = "/My/AddAddress";

        function VerifyInputs(parent) {
            var msg = null;
            var province = $(parent).find('#Province').val();
            var city = $(parent).find('#City').val();
            var district = $(parent).find('#District').val();
            var contact = $(parent).find('#Contact').val();
            var phone = $(parent).find('#Phone').val();
            var apartment = $(parent).find('#Apartment').val();
            var nearBy = $(parent).find('#NearBy').val();
            if (province == "") {
                if (msg == null) {
                    msg = "省份不能为空";
                } else {
                    msg += "<br/>省份不能为空";
                }
            }
            if (city == "") {
                if (msg == null) {
                    msg = "城市不能为空";
                } else {
                    msg += "<br/>城市不能为空";
                }
            }
            if (district == "") {
                if (msg == null) {
                    msg = "区不能为空";
                } else {
                    msg += "<br/>区不能为空";
                }
            }
            if (contact == "") {
                if (msg == null) {
                    msg = "联系人不能为空";
                } else {
                    msg += "<br/>联系人不能为空";
                }
            }
            if (phone == "") {
                if (msg == null) {
                    msg = "电话不能为空";
                } else {
                    msg += "<br/>电话不能为空";
                }
            }
            if (apartment == "") {
                if (msg == null) {
                    msg = "小区不能为空";
                } else {
                    msg += "<br/>小区不能为空";
                }
            }
            if (nearBy == "") {
                if (msg == null) {
                    msg = "靠近不能为空";
                } else {
                    msg += "<br/>靠近不能为空";
                }
            }

            return msg;
        }

        $.get(
            url,
            function (res, status) {
                if (res != null && res != "undefined") {
                    var modal = $('<div class="modal fade" id="editRuoteModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"></div>').appendTo($('body'));
                    var modalDiv = $('<div class="modal-dialog" role="document"></div>').appendTo($(modal));
                    var modalContent = $('<div class="modal-content"></div>').appendTo($(modalDiv));
                    var modalBody = $('<div class="modal-body"></div>').appendTo($(modalContent)).html(res);
                    $(modalBody).find('#btn_save_address').unbind('click');                    
                    $(modalBody).find('#btn_save_address').click(function () {                       
                        var message = VerifyInputs(modalBody);
                        if (message != null) {
                            $(modalBody).find('#submit_warning').find('p').html(message);
                            $(modalBody).find('#submit_warning').show();
                            $(modalBody).find('#submit_warning').find('button').find('span').unbind('click');                            
                            $(modalBody).find('#submit_warning').find('button').find('span').click(function () {
                                $(modalBody).find('#submit_warning').hide();
                            });
                            return;
                        }
                        var addressObj = {   };
                        addressObj.Procince = $(modalBody).find('#Province').val();
                        addressObj.City = $(modalBody).find('#City').val();
                        addressObj.District = $(modalBody).find('#District').val
                        addressObj.Phone = $(modalBody).find('#Phone').val();
                        addressObj.Contact = $(modalBody).find('#Contact').val();
                        addressObj.Apartment = $(modalBody).find('#Apartment').val();
                        addressObj.Nearby = $(modalBody).find('#NearBy').val();
                        _this.SaveAddress(addressObj, callback);
                    });
                    $(modal).modal();
                }
            }
        );
    }

    this.SaveAddress = function (address,callback) {
        if (address == null || address == undefined || typeof (address) != 'object') {
            if (callback != null && callback != undefined && typeof (callback) == 'function') {
                callback({ Status: 'ERROR', Message: '藕品地点信息不正确' });
                return;
            }
        }
        $.post(
            '/api/User/SaveAddress',
            address,
            function (res) {
                if (callback != null && callback != undefined && typeof (callback) == 'function') {
                    callback(res);
                    return;
                }
            }
        )
    }
}
UserUtil.prototype = new BaseMiOU();

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