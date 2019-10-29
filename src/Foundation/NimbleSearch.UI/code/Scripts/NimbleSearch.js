(function ($nimbleJquery, util ){
    util.jsHashChange = false;
    util.hasInitListener = false;

    util.hashListener = function (e) {
        //TO DO: handle back button
        //console.log("hashChange");
        //if ( ! util.jsHashChange ) {
        //    console.log("non js hashChange");
        //    if (tabConfig ) {
        //        var currPage = $(".selected-page").first().data("numberpage") || 0;
        //        var numResults = $("#results-select").length > 0 ? $("#results-select").val() : 12;
        //        var skipHashUpdate = true;

        //        SingletonSearcher.getInstance().intialize(new Query(currPage, numResults, tabConfig, skipHashUpdate));
        //        NimbleSearch.current().initialize(new Query(0, @pageSize.ToString(), "@tabId"), searcherConfigurations);
        //    }
        //} else {
        //    setTimeout(function(){
        //        util.jsHashChange = false;
        //    }, 300);
        //}
    };

    util.initHashListener = function(){
        if ( ! util.hasInitListener ) {
            util.hasInitListener = true;
            window.addEventListener("hashchange", util.hashListener);
        }
    };

})($nimbleJquery, window.advSearchUtil = window.advSearchUtil || {});

function unique(a) {

    for (var i = 0; i < a.length; ++i) {
        for (var j = i + 1; j < a.length; ++j) {
            if (a[i] === a[j])
                a.splice(j--, 1);
        }
    }

    return a;
};

function TabProvider(id, sitecoreId, defaultSortOptions, defaultSortDirection) {

    var urlUtility = new UrlUtility();

    this.getSitecoreId = function () {
        return sitecoreId;
    };

    this.updateState = function (query, data, callback) {
        this.initializeEvents(query, data, callback);
    };
    this.initializeEvents = function (query, data, callback) {
        if (sitecoreId == query.tabId) {
            $nimbleJquery("#" + id).addClass("active");
        }
        $nimbleJquery("#" + id).off("click").on("click", function (e) {

            if (!query.skipHashUpdate) {
                urlUtility.setHashParameter("tab", sitecoreId);
            }
            query.selectedFacets = null;
            query.sortBy = defaultSortOptions;
            query.sortDir = defaultSortDirection;
            query.pageIndex = 0;
            query.tabId = sitecoreId;
            $nimbleJquery(".search-tab").each(function (index) {
                $nimbleJquery(this).removeClass("active");
            });
            $nimbleJquery(this).addClass("active");
            callback(query);

        });
        advSearchUtil.initHashListener();
    };
}

function DataStore() {

    this.defaultNoResultMessage = "Your search returned no results. Please adjust your search and try again.";

    this.updateState = function (query, data, callback) {
        this.displayResults(query, data);
    };

    this.displayResults = function (query, data) {
        var source = document.getElementById("data-template").innerHTML;
        $nimbleJquery('#results-number').html("Showing " + data.TotalNumberOfResults + " results");

        var datastemplate = Handlebars.compile(source);
        var datahtml = datastemplate(data);

        $nimbleJquery("#data-placeholder").append(datahtml);
    };
}

function PaginationProvider() {

    this.initializeEvents = function (query, callback) {
        $nimbleJquery("#load-more").click(function () {
            query.pageIndex++;
            callback(query);
        });
    };

    this.updateState = function (query, data, callback) {
        this.display(query, data);

        if (data.Items.length) {
            if (data.CurrentPageIndex + 1 == data.TotalPages) {
                $nimbleJquery("#load-more").css("display", "none");
            } else {
                $nimbleJquery("#load-more").css("display", "inline");
            }
        }
        this.initializeEvents(query, callback);
    };

    this.display = function (query, data) {
        if (data.Items.length) {
            var source = document.getElementById("pagination-template").innerHTML;
            var paginatortemplate = Handlebars.compile(source);
            var paginatorhtml = paginatortemplate(query);

            $nimbleJquery("#pagination-placeholder").html(paginatorhtml);
        }
    };
    advSearchUtil.initHashListener();
}

function Query(pageIndex, pageSize, tabId, skipHashUpdate) {
    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.tabId = tabId;
    this.skipHashUpdate = skipHashUpdate || false;
    console.log("query", skipHashUpdate);

    this.updateFromHash = function () {
        var uriHash = window.location.hash.substr(1);

        if (uriHash != null) {
            var queryStrings = uriHash.split("&");
            if (queryStrings != null && queryStrings.length > 0) {
                var dictQueryStrings = {};
                for (var key in queryStrings) {
                    var index = queryStrings[key].indexOf("=");
                    if (index != -1) {
                        var facetName = queryStrings[key].substr(0, index);
                        var facetValues = queryStrings[key].substr(index + 1);
                        dictQueryStrings[facetName] = facetValues;
                    }
                }
                this.selectedFacets = dictQueryStrings["filter"];
            }
        }
    }

    this.execute = function () {
        $nimbleJquery("#search").trigger("beforeQueryExecution", this);
        var searchParams = {
            "Keyword": this.keyword,
            "Sort": this.sortBy,
            "Page": this.pageIndex,
            "PageSize": this.pageSize,
            "SelectedFacetsRaw": this.selectedFacets,
            "TabId": this.tabId,
        };
        var result = $nimbleJquery.ajax({
            type: "POST",
            url: "/api/nimble/search",
            data: searchParams,
            async: false,
            dataType: "json",
            success: function (data) {
                // Remove custom fields and summary (avoid potentially dangerous request error)
                var dataCopy = data ? JSON.parse(JSON.stringify(data)) : {};
                if (data && data.Items) {
                    for (var i = 0; i < dataCopy.Items.length; i++)
                    {
                        var item = dataCopy.Items[i];
                        delete item.Summary;
                        delete item.Custom;
                    }
                    dataCopy.NoResultsMessage = dataCopy.NoResultsMessage ? 'true' : '';
                }
                $nimbleJquery.ajax({
                    type: "POST",
                    url: "/api/nimble/analytics",
                    data: {
                        "SearchParameters": searchParams,
                        "SearchResponse": dataCopy
                    },
                    async: true,
                    dataType: "json"
                });
            }
        });

        return result.responseJSON;
    }
}

var SearcherConfigurations = function (noResultsNextTab) {
    this.noResultsNextTab = noResultsNextTab;
};

var NimbleSearch = (function () {
    function Searcher() {
        this.tabs = [];
        this.urlUtility = new UrlUtility();


        this.initialize = function(query, searcherConfigurations) {
            
            this.searcherConfigurations = searcherConfigurations;

            if (this.facetProvider !== null) {
                query.selectedFacets = this.urlUtility.getHashValue("filter");
                var selectedFacetsFix = this.urlUtility.getHashValue("filfix");

                if (selectedFacetsFix != null && selectedFacetsFix != "") {
                    if (query.selectedFacets == null) {
                        query.selectedFacets = selectedFacetsFix;
                    } else if (query.selectedFacets == "") {
                        query.selectedFacets = selectedFacetsFix;
                    } else {
                        var facetsArray = this.facetProvider.convertToFacetArray(query.selectedFacets);
                        var fixedFacetsArray = this.facetProvider.convertToFacetArray(selectedFacetsFix);

                        for (var fixedFacet in fixedFacetsArray) {
                            if (facetsArray[fixedFacet] != null) {
                                facetsArray[fixedFacet] = unique(facetsArray[fixedFacet].concat(fixedFacetsArray[fixedFacet]));
                            } else {
                                facetsArray[fixedFacet] = [];
                                for (var fixedFacetsValues in fixedFacetsArray[fixedFacet]) {
                                    facetsArray[fixedFacet].push(fixedFacetsValues);
                                }
                            }
                        }

                        query.selectedFacets = this.facetProvider.convertToString(facetsArray);
                    }
                }
            }
            if (this.sortProvider !== null) {
                query.sortBy = this.urlUtility.getHashValue("sortby");

                if (query.sortBy == null && searcherConfigurations.defaultSort != null) {
                    if (searcherConfigurations.defaultSort.sortBy != null) {
                        query.sortBy = searcherConfigurations.defaultSort.sortBy;
                    }
                }
            }
            if (this.keywordProvider !== null) {
                query.keyword = this.urlUtility.getHashValue("keyword");
            }
            if (this.paginationProvider !== null && this.urlUtility.getHashValue("page") !== null) {
                query.pageIndex = this.urlUtility.getHashValue("page");
            }
            if (this.urlUtility.getHashValue("pageSize") != null) {
                query.pageSize = this.urlUtility.getHashValue("pageSize");
            }
            if (this.tabs !== null && this.tabs.length > 0 && this.urlUtility.getHashValue("tab") !== null) {
                for (var index in this.tabs) {
                    if (this.tabs[index].getSitecoreId() === this.urlUtility.getHashValue("tab")) {
                        query.tabId = this.urlUtility.getHashValue("tab");
                    }
                }
            }

            this.run(query);
        }

        this.updateInterfaceBeforeExecution = function () {
            $nimbleJquery("#data-preoload-img").show();
            $nimbleJquery("#data-placeholder").hide();
            $nimbleJquery("#facets-full").hide();
            $nimbleJquery(".pagination-placeholder").hide();
        };

        this.updateInterfaceAfterExecution = function () {
            $nimbleJquery("#data-preoload-img").hide();
            $nimbleJquery("#data-placeholder").show();
            $nimbleJquery("#facets-full").show();
            $nimbleJquery(".pagination-placeholder").show();

        };

        this.run = function(query) {

            this.updateInterfaceBeforeExecution();
            var data = query.execute();
            this.updateInterfaceAfterExecution();

            $nimbleJquery("#search").trigger("tabSwitch", [query.tabId]);
            if (data !== null) {
                if (data.TotalResults === 0) {
                    //TO DO : trigger js event for empty results
                } else {
                    if (this.keywordProvider !== null && this.keywordProvider !== undefined) this.keywordProvider.updateState(query, data, this.run.bind(this));
                    if (this.facetProvider !== null && this.facetProvider !== undefined) this.facetProvider.updateState(query, data, this.run.bind(this));
                    if (this.paginationProvider !== null && this.paginationProvider !== undefined) this.paginationProvider.updateState(query, data, this.run.bind(this));
                    if (this.sortProvider !== null && this.sortProvider !== undefined) this.sortProvider.updateState(query, data, this.run.bind(this));
                    if (this.dataStore !== null && this.dataStore !== undefined) this.dataStore.updateState(query, data, this.run.bind(this));
                    if (this.tabs !== null && this.tabs.length > 0) {
                        for (var index in this.tabs) {
                            this.tabs[index].updateState(query, data, this.run.bind(this));
                        }
                    }
                    setTimeout(function(){
                        query.skipHashUpdate = false;
                    }, 100);
                }

            }

        }

    }
    var instance;
    return {
        current: function () {
            if (instance == null) {
                instance = new Searcher();
                instance.constructor = null;
            }
            return instance;
        }
    };
})();

function UrlUtility() {

    this.setHashParameter = function (paramName, paramValue) {
        var hash = location.hash;

        if (hash.indexOf(paramName + "=") >= 0) {
            var prefix = hash.substring(0, hash.indexOf(paramName));
            var suffix = hash.substring(hash.indexOf(paramName));
            suffix = suffix.substring(suffix.indexOf("=") + 1);
            suffix = (suffix.indexOf("&") >= 0) ? suffix.substring(suffix.indexOf("&")) : "";
            hash = prefix + paramName + "=" + encodeURIComponent(paramValue) + suffix;
        }
        else {
            if (hash.indexOf("#") < 0)
                hash += "#" + paramName + "=" + encodeURIComponent(paramValue);
            else
                hash += "&" + paramName + "=" + encodeURIComponent(paramValue);
        }
        window.location.hash = hash;
    }

    this.removeHashByName = function (hashName) {
        if (hashName == null && hashName == "") return;

        var hash = location.hash;
        var suffix = hash.substring(hash.indexOf(hashName));
        
        if (suffix != "") {
            hash = hash.replace(new RegExp(hashName + '=[\\w:.|%\\d]*', 'g'), "").replace("&&", "&");
            
            if (hash.endsWith("&")) {
                hash = hash.substring(0, hash.length - 1);
            }
            if (hash.startsWith("#&")) {
                hash = hash.replace("#&", "#");
            }
        }

        window.location.hash = hash;
    }

    this.getHashValue = function (name) {
        var hash = window.location.hash;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[#&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(hash);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }
}

function FacetProvider() {

    this.urlUtility = new UrlUtility();
    var initialLoad = true;

    this.convertToFacetArray = function (facets) {
        if (facets == null) return null;
        var resultFacets = [];
        var facetsArray = facets.split("||");

        for (var i = 0; i < facetsArray.length; i++) {
            var fullFacetKeyValue = facetsArray[i];

            var index = fullFacetKeyValue.indexOf(":");

            if (index > -1) {
                var facetName = fullFacetKeyValue.substr(0, index);
                var facetValues = fullFacetKeyValue.substr(index + 1);

                var facetValuesArray = facetValues.split("|");

                if (facetValuesArray != null && facetValuesArray.length > 0 && resultFacets[facetName] == null) {
                    resultFacets[facetName] = facetValuesArray;
                }
            }
        }

        return resultFacets;
    };

    this.convertToString = function (facets) {
        if (facets == null) return null;

        var stringFacets = "";
        for (var key in facets) {
            if (facets[key] == null) continue;
            if (facets[key].length < 1) continue;

            var combinedValue = facets[key][0];
            for (var i = 1; i < facets[key].length; i++) {
                combinedValue += "|" + encodeURIComponent(facets[key][i]);
            }

            if (stringFacets == "") {
                stringFacets = key + ":" + combinedValue;
            } else {
                stringFacets += "||" + key + ":" + combinedValue;
            }
        }

        return stringFacets;
    };

    this.updateHash = function (query) {
        if (query.selectedFacets != null) {
            this.urlUtility.setHashParameter("filter", query.selectedFacets);
        } else {
            this.urlUtility.removeHashByName("filter");
        }
    };

    this.display = function (data) {
        var source = document.getElementById("nimble-facets-template").innerHTML;
        var facetstemplate = Handlebars.compile(source);
        var facetshtml = facetstemplate(data);

        $nimbleJquery("#nimble-facets-placeholder").html(facetshtml);
    };

    this.build = function (filters) {
        var selectedFacets = [];

        $nimbleJquery.each(filters, function (index, value) {
            var selectedFacetKey = $nimbleJquery(value).data("facetName");

            if (selectedFacets[selectedFacetKey] == null) {
                selectedFacets[selectedFacetKey] = $nimbleJquery(value).val();
            } else {
                selectedFacets[selectedFacetKey] += '|' + $nimbleJquery(value).val();
            }
        });

        var facets = "";
        for (var key in selectedFacets) {
            if (facets == "") {
                facets = key + ":" + selectedFacets[key];
            } else {
                facets += "||" + key + ":" + selectedFacets[key];
            }
        }

        return facets;
    };

    this.updateFacets = function (query) {
        facetsArray = this.convertToFacetArray(query.selectedFacets);
        fixedFacetsArray = this.convertToFacetArray(this.urlUtility.getHashValue("filfix"));

        this.updateHtml(facetsArray, false);
        this.updateAccordion(facetsArray);
        //this.updateHtml(fixedFacetsArray, true);
    }

    this.updateAccordion = function (facetsArray) {

        if (facetsArray != null && facetsArray != []) {
            $nimbleJquery.each($nimbleJquery(".filter-checkbox"), function (index, value) {
                var selectedFacetKey = $nimbleJquery(value).data("facetName");
                if (facetsArray[selectedFacetKey] != null && facetsArray[selectedFacetKey].indexOf($nimbleJquery(value).val()) > -1) {
                    var container = $nimbleJquery(value).parents('.collapsible-item');
                    if (!container.hasClass('active')) {
                        container.addClass('active');
                    }
                }
            });
        }

        var activeFacets = $nimbleJquery(".collapsible-item .collapsible-toggle").parents('.collapsible-item.active').length;
        if (activeFacets == 0) {
            //open first
            if ($nimbleJquery(".collapsible-item .collapsible-toggle").length > 0) {
                var container = $nimbleJquery($nimbleJquery(".collapsible-item .collapsible-toggle")[0]).parents('.collapsible-item')
                if (!container.hasClass('active')) {
                    container.addClass('active');
                }
            }
        }
    }

    this.updateHtml = function (facetsArray, isDisabled) {

        var buttonsInfo = { Facets: [] };
        var facetshtml = "";
        if (facetsArray != null) {
            $nimbleJquery.each($nimbleJquery(".filter-checkbox"), function (index, value) {
                var selectedFacetKey = $nimbleJquery(value).data("facetName");
                if (facetsArray[selectedFacetKey] != null && facetsArray[selectedFacetKey].indexOf($nimbleJquery(value).val()) > -1) {
                    $nimbleJquery(value).attr('checked', true);
                    var $item = $nimbleJquery(value)[0];
                    buttonsInfo.Facets.push({ name: $item.value, category: $item.dataset.facetName });

                    if (isDisabled) {
                        $nimbleJquery(value).attr('disabled', true);
                    }
                }
            });
        }

        if (buttonsInfo.Facets.length > 0) {
            var source = document.getElementById("facets-button-template").innerHTML;
            var facetstemplate = Handlebars.compile(source);
            facetshtml = facetstemplate(buttonsInfo);
        }

        $nimbleJquery("#facets-full-buttons").html(facetshtml);

        $nimbleJquery(".facet-button").on("click", function () {
            $nimbleJquery(":checkbox[value='" +$nimbleJquery(this).val() + "']").prop("checked", "true").click();
        });

    }

    this.updateState = function (query, data, callback) {

        if (data.Facets.length == 0) {
            $nimbleJquery("#facets-full").css("display", "none");
            return;
        } else {
            $nimbleJquery("#facets-full").css("display", "block");
        }

        this.updateHash(query);
        this.display(data);
        this.updateFacets(query);
        this.initializeOnClick(query, callback);
    }

    this.initializeOnClick = function (query, callback) {

        $nimbleJquery(".filter-checkbox").click({ obj: this }, function (e) {
            $nimbleJquery("#data-placeholder").html("");
            query.selectedFacets = e.data.obj.build($nimbleJquery(".filter-checkbox:checked"));
            query.pageIndex = 0;
            callback(query);
        });

        //special case we dont want to have this event twice
        if (initialLoad) {
            initialLoad = false;
        } else {
            $nimbleJquery(".collapsible-item .collapsible-toggle, .btn-expand-all").on("click", function () {
                var container = $nimbleJquery(this).parents('.collapsible-item');
                
                if (container.hasClass('active')) {
                    container.removeClass('active');
                } else {
                    container.addClass('active');
                }
            });
        }
        
    }
}

function SortProvider() {
        
    this.urlUtility = new UrlUtility();

    this.updateHash = function (query, data) {
        if (query.sortBy != null) {
            this.urlUtility.setHashParameter("sortby", query.sortBy);
        } else if (data.DefaultSortOption != null && data.DefaultSortOption.Value != "") {
            this.urlUtility.setHashParameter("sortby", data.DefaultSortOption.Value);
        }
    }

    this.display = function (data) {
        
        var source = document.getElementById("nimble-sort-template").innerHTML;
        var facetstemplate = Handlebars.compile(source);
        var facetshtml = facetstemplate(data);

        $nimbleJquery("#nimble-sort-placeholder").html(facetshtml);

        if (data.SortOptions.length <= 1) {
            $nimbleJquery("#nimble-sort-placeholder").hide();
        } else {
            $nimbleJquery("#nimble-sort-placeholder").show();
        }
    }

    this.updateSelected = function (query, data) {

        var sortBy = this.urlUtility.getHashValue("sortby");
        if (sortBy == null && data.DefaultSortOption != null)
        {
            if(data.DefaultSortOption != null) {
                sortBy = data.DefaultSortOption.Value;
            } else if(data.SortOptions.length > 0) {
                sortBy = data.SortOptions[0].Value;
            }
        }
        //SortDirection
        query.sortBy = sortBy;
        $nimbleJquery('.nimbleSearchSort').val((sortBy));
    }

    this.updateState = function (query, data, callback) {
        this.updateHash(query, data);
        this.display(data);
        this.updateSelected(query, data);
        this.initializeOnChange(query, callback);
    }

    this.initializeOnChange = function (query, callback) {
        $nimbleJquery(".nimbleSearchSort").change({ obj: this }, function (e) {
            $nimbleJquery("#data-placeholder").html("");
            query.sortBy = $nimbleJquery('.nimbleSearchSort :selected').data("value");
            query.pageIndex = 0;
            callback(query);
        });
    }
}

function SearchBox(resultsSize) {

    var numberOfResults = resultsSize
    this.urlUtility = new UrlUtility();

    this.updateHash = function (query) {
        if (query.keyword != null && query.keyword != "") {
            this.urlUtility.setHashParameter("keyword", query.keyword);
            $nimbleJquery("#nimble-searchbox").val(query.keyword);
        } else {
            this.urlUtility.removeHashByName("keyword");
        }
    }

    this.updateState = function (query, data, callback) {
        this.updateHash(query);
        this.initializeOnSearchClicked(query, callback);
        this.initializeAutoComplete(query, callback);
    }

    this.initializeAutoComplete = function (query, callback) {
        //if (isAutocomplete != undefined && isAutocomplete == "1") {
        $nimbleJquery(function () {
            $nimbleJquery("#nimble-searchbox").autocomplete({
                source: function (request, response) {
                    var searchKeyword = $nimbleJquery("#nimble-searchbox").val();
                    $nimbleJquery.ajax({
                        type: "POST",
                        url: "/api/nimble/search",
                        datatype: "json",
                        data: {
                            "Keyword": searchKeyword,
                            "TabId": query.tabId
                        },
                        success: function (data) {
                            response($nimbleJquery.map(data.Items, function (value, key) {
                                var obj = { label: "", value: "" };
                                //$nimbleJquery.each( value.Properties, function( keylp, valuelp ) {
                                //    if(keylp.toLowerCase() == 'title') {
                                //        obj = {
                                //            label: valuelp,
                                //            value: valuelp
                                //        };
                                //    }
                                //});
                                obj = {
                                    label: value.Title,
                                    value: value.Title
                                };

                                return obj;
                            }));
                        }
                    });
                },
                minlength: 2,
                select: function (event, ui) {
                    query.pageIndex = 0;
                    query.keyword = ui.item.value;
                    callback(query);
                }
            });
        });

        // }
    }

    var clickKeyDownFunction = function (query, callback) {
        $nimbleJquery("#nimble-data-placeholder").html("");
        var searchBoxValue = $nimbleJquery("#nimble-searchbox").val();
        query.pageIndex = 0;
        query.selectedFacets = null;
        query.keyword = searchBoxValue;
        callback(query);
    }

    this.initializeOnSearchClicked = function (query, callback) {
        $nimbleJquery("#nimble-searchbox").off("keydown").on("keydown", function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                clickKeyDownFunction(query, callback);
            }
        });

        $nimbleJquery("#nimble-searchbox-button").off("click").on("click", function (e) {
            clickKeyDownFunction(query, callback);
        });
    }
}

if ($nimbleJquery('#nimble-data-placeholder').length > 0) {
    var nimbledataStore = new DataStore();

    nimbledataStore.displayResults = function (query, data) {

        $nimbleJquery('#nimble-results-number').html("Showing " + data.TotalResults + " results");
        var datastemplate = '';
        var datahtml = '';
        if (data.TotalResults > 0) {
            source = document.getElementById("nimble-data-template").innerHTML;
            datastemplate = Handlebars.compile(source);
            datahtml = datastemplate(data);
        } else {
            var selectedTab = $nimbleJquery.grep(data.Tabs, function (e) { return e.Selected == true; });

            source = this.defaultNoResultMessage;
            if (this.noResultsMessages !== null && this.noResultsMessages[query.tabId] !== null) {
                source = this.noResultsMessages[query.tabId];
            }

            datastemplate = Handlebars.compile(source);
            var tabName = "";
            if (selectedTab.length > 0) {
                tabName = selectedTab[0].Title;
            }
            datahtml = datastemplate({ keyword: query.keyword, tab: tabName });
        }

        $nimbleJquery("#nimble-data-placeholder").html(datahtml);
    };
    NimbleSearch.current().dataStore = nimbledataStore;
}


if ($nimbleJquery('#nimble-pagination-placeholder').length > 0) {
    var paginator = new PaginationProvider();

    paginator.pageMinMax = 2;
    paginator.urlUtility = new UrlUtility();
    paginator.prevPage = function (query, data, callback) {

        var $_btn_prev = $nimbleJquery(".nimble_btn_prev");

        if (data.CurrentPageIndex < 1) {
            $_btn_prev.css("display", "none");
        } else {
            $_btn_prev.css("display", "visible");
            $_btn_prev.click(function () {
                query.pageIndex--;
                callback(query);
            });
        }
    }

    paginator.display = function (query, data) {

        if (data.Paging.TotalPages > 1) {
            var source = document.getElementById("nimble-pagination-template").innerHTML;
            var paginatortemplate = Handlebars.compile(source);
            var paginatorhtml = paginatortemplate(query);

            $nimbleJquery(".nimble-pagination-placeholder").html(paginatorhtml);
        } else {
            $nimbleJquery(".nimble-pagination-placeholder").html('');
        }
    }

    paginator.nextPage = function (query, data, callback) {

        var $_btn_next = $nimbleJquery(".nimble_btn_next");

        if (data.Paging.Page >= data.Paging.TotalPages) {
            $_btn_next.css("display", "none");
        } else {
            $_btn_next.css("display", "visible");
            $_btn_next.click(function () {
                query.pageIndex++;
                callback(query);
            });
        }
    }

    paginator.pages = function (query, data, callback) {

        var $_page = $nimbleJquery(".nimble_page");

        var pageMinMaxSecondHalf = 0;
        if (this.pageMinMax - data.Paging.Page > 0) {
            pageMinMaxSecondHalf = this.pageMinMax - data.Paging.Page;
        }

        var pageMinMaxFirstHalf = 0;
        if (this.pageMinMax + data.Paging.Page >= data.Paging.TotalPages) {
            pageMinMaxFirstHalf = this.pageMinMax + data.Paging.Page - data.Paging.TotalPages + 1;
        }

        for (var i = data.Paging.Page; i > 0 && i >= (data.Paging.Page - this.pageMinMax - pageMinMaxFirstHalf); i--) {
            this.getPageNumber(i, query, data, callback).prependTo($_page);
        }

        for (var i = data.Paging.Page + 1; i < data.Paging.TotalPages && i <= (data.Paging.Page + this.pageMinMax + pageMinMaxSecondHalf); i++) {
            this.getPageNumber(i, query, data, callback).appendTo($_page);
        }

    }

    paginator.getPageNumber = function (index, query, data, callback) {
        page = document.createElement('input');

        if ((data.Paging.Page) == index) {
            $nimbleJquery(page).addClass("selected-page");
        }

        return $nimbleJquery(page).attr("type", "button")
            .val(index)
            .click(function () {
                query.pageIndex = $nimbleJquery(this).val();
                callback(query);
            });
    }

    paginator.initializeEvents = function (query, data, callback) {
        this.pages(query, data, callback);
        this.prevPage(query, data, callback);
        this.nextPage(query, data, callback);
    }

    paginator.updateState = function (query, data, callback) {
        this.updateHash(query);
        this.display(query, data);
        this.initializeEvents(query, data, callback);
    }

    paginator.updateHash = function (query) {
        this.urlUtility.setHashParameter("page", query.pageIndex);
    }
    NimbleSearch.current().paginationProvider = paginator;
}

if ($nimbleJquery('#nimble-facets-placeholder').length > 0) {
    NimbleSearch.current().facetProvider = new FacetProvider();
}

if ($nimbleJquery('#nimble-searchbox').length > 0) {
   
    NimbleSearch.current().keywordProvider = new SearchBox(10);
} 

if ($nimbleJquery('#nimble-sort-placeholder').length > 0) {

    NimbleSearch.current().sortProvider = new SortProvider();
} 