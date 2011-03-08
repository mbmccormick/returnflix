<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="returnflix.Default"
    MasterPageFile="~/Common/Layout.Master" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:TextBox ID="ddEmailAddress" runat="server" title="Email Address" Width="300" />
    <br />
    <asp:Button ID="uxAuthorize" runat="server" Text="Log In with your Netflix Account"
        OnClick="uxAuthorize_Click" Style="margin-top: 10px;" />
    <div class="section">
        <b>What is Returnflix?</b>
        <p>
            Returnflix is a simple service that helps you make the most of your Netflix subscription
            by ensuring that you return your Netflix movies in a timely manner. Returnflix monitors
            your rental activity and will send you an email reminder if you forget to return
            a movie to Netflix.</p>
        <br />
        <b>How does this work?</b>
        <p>
            To sign up for Returnflix, just enter your email address above and click the "Log
            In with your Netflix Account" button to get started. You'll then be taken to the
            Netflix website, where you'll authorize Returnflix to access your rental activity.
            Returnflix will send you an email reminder if you've had a movie for more than one
            week.</p>
    </div>
</asp:Content>
