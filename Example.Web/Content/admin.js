
//This wires up the Mandatory Data Annotation
(function ($) {
    $.validator.unobtrusive.adapters.addBool("mandatory", "required");
} (jQuery));

function init_admin_jquery(scope) {

    //We're the help-inline and error classes where needed so that the mvc errors are displayed cleanly with bootstrap
    scope.find('span.field-validation-valid, span.field-validation-error').each(function () {
        $(this).addClass('help-inline');
    });
    scope.find('form').submit(function () {
        if ($(this).valid()) {
            $(this).find('div').each(function () {
                if ($(this).find('span.field-validation-error').length == 0) {
                    $(this).removeClass('error');
                }                
            });
        }
        else {
            $(this).find('div').each(function () {
                if ($(this).find('span.field-validation-error').length > 0) {
                    $(this).addClass('error');
                }                
            });
        }
    });
    scope.find('form').each(function () {
        $(this).find('div').each(function () {
            if ($(this).find('span.field-validation-error').length > 0) {
                $(this).addClass('error');
            }
        });
    });
}

$(document).ready(function () {
    init_admin_jquery($(this));
});
