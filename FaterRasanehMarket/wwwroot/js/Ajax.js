$(function () {
    var placeholder = $("#modal-placeholder");
    $(document).on('click', 'button[data-toggle="ajax-modal"]', function () {
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () {
                ShowLoading();
            },
            error: function () {
                $("body").preloader('remove');
                ShowSweetErrorAlert();
            }
        }).done(function (Result) {
            $("body").preloader('remove');
            placeholder.html(Result);
            try { GetReady(); } catch{ }
            placeholder.find('.modal').modal('show');
        });
    });

    $(document).on('click', 'a[data-toggle="ajax-modal"]', function () {
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () {
                ShowLoading();
            },
            complete: function () {
                $("body").preloader('remove');
            },
            error: function () {
                ShowSweetErrorAlert();
            }
        }).done(function (result) {
            placeholder.html(result);
            try { GetReady(); } catch{ }
            placeholder.find('.modal').modal('show');
            $("body").preloader('remove');
        });
    });
    placeholder.on('click', 'button[data-save="modal"]', function () {
        var form = $(this).parents(".modal").find('form');
        var actionUrl = form.attr('action');
        if (form.length == 0) {
            form = $(".card-body").find('form');
            actionUrl = form.attr('action') + "/" + $(".modal").attr('id');
        }
        var dataToSend = new FormData(form.get(0));
        $.ajax({
            url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, mimeType: "multipart/form-data",
            beforeSend: function () {
                ShowLoading();
            },
            error: function () {
                $("body").preloader('remove');
                ShowSweetErrorAlert();
            }
        }).done(function (data) {
            var newBody = $(".modal-body", data);
            var newFooter = $(".modal-footer", data);
            placeholder.find(".modal-body").replaceWith(newBody);
            placeholder.find(".modal-footer").replaceWith(newFooter);
            var IsValid = newBody.find("input[name='IsValid']").val() === "True";
            if (IsValid) {
                $.ajax({
                    url: '/Admin/Base/Notification',
                    error: function () {
                        ShowSweetErrorAlert();
                    }
                }).done(function (notification) {
                    ShowSweetSuccessAlert(notification);
                });
                try {
                    $table.bootstrapTable('refresh')
                }
                catch{
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
                placeholder.find(".modal").modal('hide');
            }
            else {
                try { GetReady(); } catch{ }
            }
            $("body").preloader('remove');
        });

    });
});


function ShowSweetErrorAlert() {
    Swal.fire({
        type: 'error',
        title: 'خطایی رخ داده است !!!',
        text: 'لطفا تا برطرف شدن خطا شکیبا باشید.',
        confirmButtonText: 'بستن'
    });
}

function ShowLoading() {
    $("body").preloader({ text: "لطفا صبر کنید..." });
}

function ShowSweetSuccessAlert(message) {
    Swal.fire({
        position: 'top-middle',
        type: 'success',
        title: message,
        showConfirmButton: false,
        timer: 2000
    })
}
function ShowSweetSuccessButtonAlert(message) {
    Swal.fire({
        position: 'top-middle',
        confirmButtonText: 'تایید',
        type: 'success',
        title: message,
        showConfirmButton: true,
    })
}


function Exit() {
    try {
        ShowLoading();
    } catch{ }
    var placeholder = $("#modal-placeholder");
    $.ajax({
        url: '/Logout',
        method: 'get'
    }).done(function (Result) {
        try {
            $("body").preloader('remove');
        } catch{ }
        placeholder.html(Result);
        placeholder.find('.modal').modal('show')
    })
}
$(document).on('click', 'input[data-save="Confirm"]', function () {
    var Id = $(this).val();
    var actionUrl = $(this).attr('data-Url') + Id
    $.ajax({
        url: actionUrl, type: "post",
        dataType: false,
        error: function () {
            ShowSweetErrorAlert();
        }
    })
});

function readURL(input, Id) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(Id).attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
        document.getElementById(Id.id + "in").value = "null";
    }
}
/*---------------------------------------------------
   AddProductToCart
----------------------------------------------------- */
$("#button-cart").on("click", function () {
    var count = $("#input-quantity").val();
    $.ajax({
        url: this.getAttribute("url") + '&count=' + count,
        error: function (Result) {
            FaildAlert(Result.responseText);
        }
    }).done(function (Result) {
        SuccessAlert(Result);
        $('#userCart').load('/CartAndOrder/UserCart');
    })
});
function addToCart(obj) {
    $.ajax({
        url: obj.getAttribute("url") + '&count=1',
        error: function (Result) {
            FaildAlert(Result.responseText);
        }
    }).done(function (Result) {
        SuccessAlert(Result);
        $('#userCart').load('/CartAndOrder/UserCart');
    });
};
function removeFromCart(obj, reload = false) {
    $.ajax({
        url: obj.getAttribute("url"),
        error: function (Result) {
            FaildAlert(Result.responseText);
        }
    }).done(function (Result) {
        SuccessAlert(Result);
        if (reload == false)
            $('#userCart').load('/CartAndOrder/UserCart');
        else {
            setTimeout(function () {
                location.reload();
            }, 2000);        }
    });
};
function updateCart(obj) {
    var count = $("#quantity" + obj.getAttribute('row')).val();
    $.ajax({
        url: obj.getAttribute("url") + '&count=' + count,
        error: function (Result) {
            FaildAlert(Result.responseText);
        }
    }).done(function (Result) {
        SuccessAlert(Result);
        setTimeout(function () {
            location.reload();
        }, 2000);
    });
};
function SuccessAlert(text) {
    window.createNotification({
        closeOnClick: false,
        displayCloseButton: false,
        positionClass: 'nfc-bottom-right',
        showDuration: 2000,
        theme: 'success'
    })({
        title: '',
        message: text
    });
}
function FaildAlert(text) {
    window.createNotification({
        closeOnClick: false,
        displayCloseButton: false,
        positionClass: 'nfc-bottom-right',
        showDuration: 3000,
        theme: 'error'
    })({
        title: '',
        message: text
    });
}
/*---------------------------------------------------
   addOrRemoveCodeFromCart
----------------------------------------------------- */
$("#discount-buttom").on("click", function () {
    var Code = $("#DiscountCode").val();
    $.ajax({
        url: '/CartAndOrder/AddOrRemoveDiscountFromCart?Code=' + Code,
        error: function (Result) {
            FaildAlert(Result.responseText);
        }
    }).done(function (Result) {
        SuccessAlert(Result);
        setTimeout(function () {
            location.reload();
        }, 2000);
    });
});
$('.radio-group .radio .address').click(function () {
    $(this).parent().parent().find('.radio').removeClass('selected');
    $(this).parent().addClass('selected');
    $.ajax({
        url: '/Account/SetDefualtAddress?addressId=' + $(this).parent().attr('data-value'),
        error: function (Result) {
            FaildAlert(Result.responseText);
        }
    }).done(function (Result) {
        SuccessAlert(Result);
    });
});
function editAddress(id) {
    $("#" + id).toggle();
}
function deleteAddress(id) {
    $.ajax({
        url: '/Account/DeleteAddress?addressId=' + id,
        error: function (Result) {
            FaildAlert(Result.responseText);
        }
    }).done(function (Result) {
        SuccessAlert(Result);
        setTimeout(function () {
            location.reload();
        }, 2000);
    });
};
function changeOrderStatus(obj) {
    $.ajax({
        url: '/Admin/Order/ChangeOrderStatus?orderId=' + obj.id + '&orderStatus=' + obj.value,
        error: function (Result) {
            if (Result.responseText == "NeedToSetShippingTime") {
                $.ajax({
                    url: '/Admin/Order/RenderOrder?orderId=' + obj.id + '&orderStatus=' + obj.value,
                    beforeSend: function () {
                        ShowLoading();
                    },
                    error: function () {
                        $("body").preloader('remove');
                        ShowSweetErrorAlert();
                    }
                }).done(function (Result) {
                    var placeholder = $("#modal-placeholder");
                    $("body").preloader('remove');
                    placeholder.html(Result);
                    try { GetReady(); } catch{ }
                    placeholder.find('.modal').modal('show');
                });
            }
            else
                ShowSweetErrorAlert();
        }
    }).done(function (Result) {
        ShowSweetSuccessAlert(Result);
    });
};
/*---------------------------------------------------
   CloseOrOpenShop
----------------------------------------------------- */
$("#closeopen-buttom").on("click", function () {
    $.ajax({
        url: '/admin/Setting/CloseOrOpenShop',
        error: function () {
            ShowSweetErrorAlert();
        },
    }).done(function (Result) {
        ShowSweetSuccessAlert(Result);
        setTimeout(function () {
            location.reload();
        }, 2000);
    });
});