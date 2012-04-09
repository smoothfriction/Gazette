(function (jQuery) {
    jQuery.ajaxSetup({ async: false });
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shCore.js"));
    SyntaxHighlighter.all({ tagname: "pre" });
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushPlain.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushPowerShell.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushJScript.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushCSharp.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushCss.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushDiff.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushJScript.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushPhp.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushSql.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushXml.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushJava.js"));
    jQuery.getScript($.url("Scripts/SyntaxHighlighter/shBrushPython.js"));
    jQuery.ajaxSetup({ async: true });
})($);

