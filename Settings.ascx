<%@ Control Language="C#" AutoEventWireup="true" Codebehind="Settings.ascx.cs" Inherits="DotNetNuclear.DNN.Authentication.Reddit.Settings, DotNetNuclear.Authentication.Reddit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:Label ID="lblHelp" runat="Server" resourcekey="RedditAuthHelp" CssClass="Normal" />
<dnn:propertyeditorcontrol id="SettingsEditor" runat="Server" 
    editcontrolwidth="450px" 
    labelwidth="350px" 
    width="100%" 
    editcontrolstyle-cssclass="NormalTextBox" 
    helpstyle-cssclass="Help" 
    labelstyle-cssclass="SubHead" 
    editmode="Edit"
    SortMode="SortOrderAttribute"
    />