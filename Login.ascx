<%@ Control Language="C#" AutoEventWireup="true" Codebehind="LogIn.ascx.cs" Inherits="DotNetNuclear.DNN.Authentication.Reddit.Login, DotNetNuclear.Authentication.Reddit" %>

<asp:Panel ID="pnlRedditLoginWrapper" CssClass="redditLoginpanel" DefaultButton="loginButton" runat="server">
    <asp:LinkButton runat="server" ID="loginButton" CausesValidation="False" Text="Sign in with Reddit">
        <div class="loginButtonContentWrapper">
            <div class="loginButtonIcon">
                <div>
                    <img src="<%=base.ControlPath %>/resources/images/reddit.png" alt="Reddit"/>
                </div>
            </div>
            <span class="loginButtonTitle">
                <span>Sign in with Reddit</span>
            </span>
        </div>    
    </asp:LinkButton>
</asp:Panel>