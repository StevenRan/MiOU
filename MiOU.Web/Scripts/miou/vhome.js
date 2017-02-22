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
        var url = "/My/AddressForm";
        $('#address_book_ajax_modal').remove();
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
                    var modal = $('<div class="modal fade" id="address_book_ajax_modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel"></div>').appendTo($('body'));
                    var modalDiv = $('<div class="modal-dialog" role="document"></div>').appendTo($(modal));
                    var modalContent = $('<div class="modal-content"></div>').appendTo($(modalDiv));
                    var modalBody = $('<div class="modal-body"></div>').appendTo($(modalContent));
                    $(modalBody).html(res);
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
                        $(modalBody).find('#submit_warning').hide();
                        var addressObj = {};                       
                        addressObj.AddressId = $('#AddressForm').find('#Id').val();
                        addressObj.Province = $('#AddressForm').find('#Province').val();
                        addressObj.City = $('#AddressForm').find('#City').val();
                        addressObj.District = $('#AddressForm').find('#District').val();
                        addressObj.Phone = $('#AddressForm').find('#Phone').val();
                        addressObj.Contact = $('#AddressForm').find('#Contact').val();
                        addressObj.Apartment = $('#AddressForm').find('#Apartment').val();
                        addressObj.Nearby = $('#AddressForm').find('#NearBy').val();                        
                        SaveAddress(addressObj, callback, modal);
                        
                    });
                    $(modal).modal();
                }
            }
        );
    }

    function SaveAddress(address, callback, modal) {
        if (address == null || address == 'undefined' || typeof (address) != 'object') {
           if (callback != null && callback != undefined && typeof (callback) == 'function') {
                callback({ Status: 'ERROR', Message: '藕品地点信息不正确' });
                return;
            }
        }        
        $.post(
            '/api/User/SaveAddress',
            { AddressId: address.AddressId, Province: address.Province, City: address.City, District: address.District, Phone: address.Phone, Contact: address.Contact, Apartment: address.Apartment, NearBy: address.Nearby },
            function (res) {
                $(modal).modal('hide').data('bs.modal', null);
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