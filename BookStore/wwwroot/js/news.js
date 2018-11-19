$.ajax({
    url:
        'https://newsapi.org/v2/everything?' +
        'q=Books&' +
        'from=2018-11-01&' +
        'apiKey=4746d814d69044f19f976956f2c1bc07&' +
        'pageSize=10',
    method: "GET",
    error: function () {
        console.log("fucked");
    },
    success: function (data) {
        processData(data);
    }
});

function processData(data) {
    //console.log(data)
    var articleItems = [];
    articleItems = data.articles;
    for (i in articleItems) {
        var author = data.articles[i].author;
        var title = data.articles[i].title;
        var description = data.articles[i].description;
        var artUrl = data.articles[i].url;

        var itemHtml =
            "<div class='item'>" +
            "<div class='carousel-content'>" +
            "<div><b>" + author + ":  " + title +
            "</b>" +
            "<a href=" + artUrl + "><div class='description'>" + description + "</div></a> "
        "</div>" +
            "</div>" +
            "</div>"

        //console.log(itemHtml);
        $(".carousel-inner").append(itemHtml);
        //console.log(artUrl);
    }
}