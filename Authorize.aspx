<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Authorize.aspx.cs" Inherits="returnflix.Authorize"
    MasterPageFile="~/Common/Layout.Master" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="welcome">
        <asp:Label ID="uxAuthorizeMessage" runat="server" /><br />
        <br />
        <br />
        <div id="smile">
            :)</div>
        <br />
        <br />
        Tired of the reminders? Click
        <asp:HyperLink ID="ddUnsubscribe" runat="server" Text="here" />
        to unsubscribe.
    </div>
</asp:Content>
