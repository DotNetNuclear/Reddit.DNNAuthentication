<%@ Control Language="C#" AutoEventWireup="true" Codebehind="LogIn.ascx.cs" Inherits="DotNetNuclear.DNN.Authentication.Reddit.Login, DotNetNuclear.Authentication.Reddit" %>

<asp:Panel ID="pnlRedditLoginWrapper" CssClass="reddit-loginpanel" DefaultButton="loginButton" runat="server">
    <asp:LinkButton runat="server" ID="loginButton" CausesValidation="False" Text="Go to Reddit Login">
    </asp:LinkButton>
</asp:Panel>
