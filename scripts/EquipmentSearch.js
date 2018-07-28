var currentList = {}; //current list to show
var searchType; //type of search

//when page is loaded
$(document).ready(function () {

    //defaults search type
    searchType = 0;
    selectSearchType(searchType);

    //runs GET action for the full list of items
    $.ajax({
        type: "GET",
        dataType: "json",
        url: "api/Equipment",
        success: function (result) {
            //if it is a successful request set the current list to the result of the server
            console.info("Success");
            currentList = result;
          
            //show the items
            if (result != null)
            {
                showItems();
            }
        }
    });

    //usability for ENTER key when serching
    $("#searchBarId").keyup(function (event) {
        if (event.keyCode == 13)
        {
            //gets the query from the search bar and searches
            var idQuery = $("#searchBarId").val();
            search(idQuery);
        }
    })

    //submit button
    $("#searchButton").click(function () {
        var idQuery = $("#searchBarId").val();
        search(idQuery);
    })
});

//changes search type and some css for usability
function selectSearchType(type)
{
    var unitButton = $("#btnUnitNoId");
    var typeButton = $("#btnTypeId");

    switch(type)
    {
        case 0:
            unitButton.css('background-color', 'red');
            unitButton.css('color', 'white');

            typeButton.css('background-color', 'white');
            typeButton.css('color', 'black');

            searchType = 0;

            console.info(searchType);
            break;

        case 1:
            unitButton.css('background-color', 'white');
            unitButton.css('color', 'black');

            typeButton.css('background-color', 'red');
            typeButton.css('color', 'white');

            searchType = 1;

            console.info(searchType);

            break;


    }
}

//gets the ul HTML element and adds li HTML elements for each ListItem along
function showItems()
{
    var $list = $("#equipmentListItems").empty();

    //cycles the list and adds the item data to the li
    for (var i = 0; i < currentList.length; i++)
    {
        var item = currentList[i];
        var $li = $("<li>").html("<b>" + item.UnitId + "</b>" + "<br />" + item.ItemId + "<br />" + item.Description);


        $li.appendTo($list); //adds the list item to the ul

    }
}

//function for search
function search(query)
{
    //creates a client side list model
    var searchModel = { sTerm: query, sType: searchType }

    //POST action
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "api/Equipment",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(searchModel), //turns the json object to a string
        success: function (result) {
            console.info(result);
            //returns the new list and shows the items
            currentList = result;
            showItems();
        }
    });

}
