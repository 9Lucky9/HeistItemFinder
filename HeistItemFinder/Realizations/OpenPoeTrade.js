(function (window, document, undefined) {

    const poeSite = "https://www.pathofexile.com/trade/search/Crucible";
    const poeApi = "https://ru.pathofexile.com/api/trade/search/Crucible";

    var importantStuff = window.open('', '_blank');
    importantStuff.location.href = poeSite;
    importantStuff.onload = init;

    function init() {
        var testPayload = { "query": { "status": { "option": "online" }, "name": "Копия Причуды Атзири", "type": "Амулет из ракушек", "stats": [{ "type": "and", "filters": [] }] }, "sort": { "price": "asc" } };
        var response = importantStuff.fetch(poeApi, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(testPayload)
        })
    }

})(window, document, undefined);


