$(document).ready(function () {
    // Populate City dropdown
    $.get("/Patient/GetCities", function (data) {
        $.each(data, function (index, item) {
            $("#cityDropdown").append(new Option(item.text, item.value));
        });
    });

    // Handle City selection change
    $("#cityDropdown").change(function () {
        var cityId = $(this).val();
        if (cityId !== "") {
            $.get("/Patient/GetSuburbsByCity?cityId=" + cityId, function (data) {
                $("#suburbDropdown").empty().append('<option value="">-- Select Suburb --</option>');
                $.each(data, function (index, item) {
                    $("#suburbDropdown").append(new Option(item.text, item.value));
                });
            });
        } else {
            $("#suburbDropdown").empty().append('<option value="">-- Select Suburb --</option>');
        }
    });
});