![logo](https://github.com/luckyllama/LuckyAssetManager/raw/master/Resources/Logo.png)

## Install

It's easiest to install via nuget. 

Run the following command in the Package Manager Console

    PM> Install-Package LuckyAssetManager

## Example Usage (Mvc3 / Razor)

In the _layout.cshtml: 

```html
    <head>
        @{Assets.Css("~/Content/site-special.css", "Special").Add();}
        
        @Assets.RenderCss(assetGroup: "Special")
        
        @{
            Assets.MasterCss("~/Content/site.css").Add();
            Assets.MasterCss("~/Content/site.print.css").ForMediaType("print").Add();
            Assets.MasterCss("~/Content/site2.css").Add();
            Assets.MasterCss("~/Content/ie7.css").ForIE(IE.Equality.EqualTo, IE.Version.IE7).Add();
            Assets.Css("http://www.example.com/example.css").Add();
        }
    
        @Assets.RenderCss()
    </head>
    <body>
        @{
            Assets.MasterJavascript("~/Scripts/jquery-1.5.1.js").WithAlternatePath("cdn", "http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.5.1.js").Add();
            Assets.MasterJavascript("~/Scripts/jquery-ui-1.8.11.js").Add();
            Assets.MasterJavascript("~/Scripts/jquery.unobtrusive-ajax.js").Add();
            Assets.MasterJavascript("~/Scripts/jquery.validate.js").Add();
            Assets.MasterJavascript("~/Scripts/jquery.validate.unobtrusive.js").Add();
        }

        @Assets.RenderJavascript()
    </body>
```

And in any .cshtml file (page or partial): 

```html
    <!-- place anywhere on the page -->
    @{
        Assets.Css("~/Content/site.css").Add();
        Assets.Css("~/Content/home-index.css").Add();
        Assets.Css("http://www.example.com/example.css").Remove();
        Assets.Javascript("~/Scripts/jquery-1.5.1.js").Add();
        Assets.Javascript("~/Scripts/jquery-ui-1.8.11.js").Add();
    }
```

The method Assets.MasterCss is only slightly different than the method Assets.Css. The former is simply always rendered before the latter. This insures the correct ordering is preserved since the .net engine actually renders the aspx page first, then any user controls, then the master page last.

### Output

With both combining and minimization off: 

```html
    <head>
        <link href="/Content/site-special.css" media="all" rel="stylesheet" type="text/css" />
        <link href="/Content/site.css" media="all" rel="stylesheet" type="text/css" />
        <link href="/Content/site.print.css" media="print" rel="stylesheet" type="text/css" />
        <link href="/Content/site2.css" media="all" rel="stylesheet" type="text/css" />
        <!--[if  IE 7]>
        <link href="/Content/ie7.css" media="all" rel="stylesheet" type="text/css" />
        <![endif]-->
        <link href="/Content/home-index.css" media="all" rel="stylesheet" type="text/css" />
        <link href="/Content/test-user-control-1.css" media="all" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <script src="/Scripts/jquery-1.5.1.js" type="text/javascript"></script>
        <script src="/Scripts/jquery-ui-1.8.11.js" type="text/javascript"></script>
        <script src="/Scripts/jquery.unobtrusive-ajax.js" type="text/javascript"></script>
        <script src="/Scripts/jquery.validate.js" type="text/javascript"></script>
        <script src="/Scripts/jquery.validate.unobtrusive.js" type="text/javascript"></script>
    </body>
```

There are 12 http calls. Note there are no duplicates and the call to example.css was removed from the master page by a child page (or control). The total size of the javascript is __645.2 KB__.

With combining on:

```html
    <head>
        <link href="/Content/site-special.css" media="all" rel="stylesheet" type="text/css" />
        <link href="assets.axd?type=Css&key=1618531572" media="all" rel="stylesheet" type="text/css" />
        <link href="/Content/site.print.css" media="print" rel="stylesheet" type="text/css" />
        <!--[if  IE 7]>
        <link href="/Content/ie7.css" media="all" rel="stylesheet" type="text/css" />
        <![endif]-->
    </head>
    <body>
        <script src="assets.axd?type=Javascript&key=55890042" type="text/javascript"></script>
    </body>
```
  
There are now only 4 http calls. Note that similar groups are combined together but leaves css with a different media type or IE comment and css put into a different asset group are left out. If there's only one asset being rendered, it bypasses the http handler. 

With combining and minimization on: 

```html
    <head>
        <link href="assets.axd?type=Css&key=-1282283045" media="all" rel="stylesheet" type="text/css" />
        <link href="assets.axd?type=Css&key=1618531572" media="all" rel="stylesheet" type="text/css" />
        <link href="assets.axd?type=Css&key=966965287" media="print" rel="stylesheet" type="text/css" />
        <!--[if  IE 7]>
        <link href="assets.axd?type=Css&key=-1587408507" media="all" rel="stylesheet" type="text/css" />
        <![endif]-->
    </head>
    <body>
        <script src="assets.axd?type=Javascript&key=55890042" type="text/javascript"></script>
    </body>
```
  
There are still only 4 http calls but they are all being compressed. The javascript has been compressed to __340.5 KB__. A compression rate of __52.8%__.

Alternate Paths

You'll notice we added jQuery in the following way:

    @{
        Assets.MasterJavascript("~/Scripts/jquery-1.5.1.js").WithAlternatePath("cdn", "http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.5.1.js").Add();
    }

If we edit the web.config and set an alternate path like so: 

```xml
    <lucky>
        <assetManager debug="false">
            <css minimize="true" combine="true" />
            <javascript minimize="true" combine="true" alternateName="cdn" />
        </assetManager>
    </lucky>
```

Our html output will use the cdn instead of the local copy:
  
```html
    <body>
        <script src="http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.5.1.js" type="text/javascript"></script>
        <script src="assets.axd?type=Javascript&key=55890042" type="text/javascript"></script>
    </body>
```