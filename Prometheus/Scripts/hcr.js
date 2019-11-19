var HCR = function () {
    var show = function () {
        $(function () {
            var hcrkey = $('#hcrkey').val();
            if (hcrkey == '')
            { return false; }

            $.post('/Domino/MiniPIP/ShowHCRData', {
                hcrkey: hcrkey
            },
            function (output) {
                $('#hcrcontent').empty();
                $.each(output.datalist, function (idx, val) {
                    var appendstr = "<tr>";
                    appendstr += "<td>" + val.k + "</td>";
                    appendstr += "<td>" + val.v + "</td>";
                    appendstr += "</tr>";
                    $('#hcrcontent').append(appendstr);
                });
            });

        })
    }

    return {
        init: function () {
            show();
        }
    }
}();