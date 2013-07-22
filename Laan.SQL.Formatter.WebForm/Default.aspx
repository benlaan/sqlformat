<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Laan.Sql.Formatter.Web.Formatter" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Laan SQL Formatter (alpha)</title>
    <link href="/Content/prettify.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        
    html, body {
      margin: 0;
      padding: 0;
    } 

    </style>
    <script type="text/javascript" src="/Scripts/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="/Scripts/prettify.js"></script>

    <script type="text/javascript">
        $(document).ready(function() { prettyPrint(); });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-top:10px; margin-bottom:10px; background-color:White; border-width: 1px">
        <span>Want the code? <a href="http://github.com/benlaan/sqlformat">have a look</a></span>
        <br/>
        <span>Found a bug? <a href="http://github.com/benlaan/sqlformat/issues">log it</a></span>
    </div>
    <div>
        Input SQL
        <asp:TextBox class="code prettyprint lang-sql" ID="sqlInput" runat="server" style="width: 100%; height:180px" TextMode="MultiLine"></asp:TextBox>
    </div>
    <div style="text-align:right">
        <asp:Button ID="btnConvert" runat="server" Text="Convert" onclick="btnConvert_Click" />
    </div>
    <div>
    <% if ( IsPostBack ) { %>
        <div style="padding-bottom: 15px">
            Time Taken:
            <asp:Label runat="server" ID="timeTaken"></asp:Label>
        </div>
        <% } %>
        Output SQL
        <pre class="code prettyprint" style="width: 100%">
            <asp:Repeater ID="sqlOutput" runat="server">
                <ItemTemplate><%# Container.DataItem %></ItemTemplate>
            </asp:Repeater>     
        </pre>
    </div>
</asp:Content>
