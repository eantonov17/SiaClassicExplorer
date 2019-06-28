<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SiaClassicExplorer._Default" EnableViewState="False" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1>SiaClassic blockchain explorer</h1>
    <%--<h3 style="color: red">Code update in progress. Last block 212577</h3>--%>
    <p style="color: red">Development in progress. Use with caution. No correctness and no usability of the data is assumed.</p>
    <p style="color: red">Data available to this site is loaded from block 179,000 and up.</p>
    <hr />

    Enter block height (as a positive number), block depth (as a negative number), tx id, or address:
    <br />
    <input type="text" name="search" size="80" />
    <br />
    <asp:Button ID="SearchButton" runat="server" Text="Search" />
    <br />
    <asp:Panel ID="NothingFoundPanel" runat="server" Visible="false">
        <p><strong style="color: red; background-color: yellow;">Nothing found for this input. Please try a different value.</strong></p>
        <br />
    </asp:Panel>
    <hr />

    <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
        <p><strong style="color: red; background-color: yellow;">An error happen. Working on it. Please try later.</strong></p>
        <p> <asp:Label ID="ErrorLabel" runat="server" /></p>
        <br />
    </asp:Panel>

    <asp:Panel ID="BlockListPanel" runat="server" Visible="false">
        <h3>Showing 20 blocks, depth
                <asp:Label ID="BlockListOffsetLabel" runat="server" />
            from top of db</h3>
        <asp:ListView ID="BlockListView" runat="server">
            <LayoutTemplate>
                <table runat="server" id="tblDepartments" width="640px" style="padding: 6px;">
                    <tr>
                        <th style="padding: 6px;">Height</th>
                        <th style="padding: 6px;">Time UTC</th>
                        <th style="padding: 6px;">Miner Payouts</th>
                        <th style="padding: 6px;">Tx Count</th>
                    </tr>
                    <tr runat="server" id="itemPlaceholder" />
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server">
                    <td style="padding: 6px;">
                        <a href="/Default?block=<%#Eval("height")%>"><%#Eval("height")%></a>
                    </td>
                    <td style="padding: 6px;">
                        <%#Eval("time") %>
                    </td>
                    <td style="padding: 6px;">
                        <%#Eval("minerpayouts") %>
                    </td>
                    <td style="padding: 6px;">
                        <%#Eval("tx_count") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </asp:Panel>

    <asp:Panel ID="BlockViewPanel" runat="server" Visible="false">
        <h3>Block
            <asp:Label ID="BlockHeightLabel" runat="server" />
        </h3>
        <p>
            <asp:Label ID="BlockTimeLabel" runat="server" />
        </p>
        <asp:ListView ID="BlockViewListView" runat="server" Visible="true">
            <LayoutTemplate>
                <table runat="server" width="640px">
                    <tr runat="server" id="itemPlaceholder" />
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server">
                    <td style="padding: 1px;">
                        <a href="/Default?tx=<%#Eval("id")%>"><strong>Transaction <%#Eval("id")%></strong></a><div class="row">
                            <div class="col-xs-2">
                                inputs=<%#Eval("siacoininputs")%>
                            </div>
                            <div class="col-xs-2">
                                outputs=<%#Eval("siacoinoutputs")%>
                            </div>
                            <div class="col-xs-2">
                                contracts=<%#Eval("filecontracts")%>
                            </div>
                            <div class="col-xs-2">
                                revisions=<%#Eval("filecontractrevisions")%>
                            </div>
                            <div class="col-xs-2">
                                proofs=<%#Eval("storageproofs")%>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-2">
                                fundinputs=<%#Eval("siafundinputs")%>
                            </div>
                            <div class="col-xs-2">
                                fundoutputs=<%#Eval("siafundoutputs")%>
                            </div>
                            <div class="col-xs-2">
                                minerfees=<%#Eval("minerfees")%>
                            </div>
                            <div class="col-xs-2">
                                arbitrary=<%#Eval("arbitrarydata")%>
                            </div>
                            <div class="col-xs-2">
                                signatures=<%#Eval("transactionsignatures")%>
                            </div>
                        </div>

                        <%--                    id,siacoininputs,siacoinoutputs,
                    filecontracts,filecontractrevisions,storageproofs,"
                + "siafundinputs,siafundoutputs,minerfees,arbitrarydata
                    ,transactionsignatures--%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </asp:Panel>

    <asp:Panel ID="TxViewPanel" runat="server" Visible="false">
        <h3>Transaction
            <asp:Label ID="TxViewIdLabel" runat="server"></asp:Label></h3>
        <p>
            <asp:HyperLink ID="TxViewHeightHyperLink" runat="server" />
            @ 
            <asp:Label ID="TxViewTimeLabel" runat="server" />
        </p>
        <asp:ListView ID="TxViewListView" runat="server">
            <LayoutTemplate>
                <table runat="server" width="640px">
                    <tr>
                        <th style="padding: 6px;">Output Address</th>
                        <th style="padding: 6px;">SCC</th>
                    </tr>

                    <tr runat="server" id="itemPlaceholder" />
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server">
                    <td style="padding: 6px;"><a href="/Default?addr=<%#Eval("unlockhash")%>"><%#Eval("unlockhash")%></a></td>
                    <td style="padding: 6px;"><%#Eval("value")%></td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </asp:Panel>

    <asp:Panel ID="AddressViewPanel" runat="server" Visible="False">
        <h3>Address
            <asp:Label ID="AddressViewLabel" runat="server" /></h3>
        <p>
            <asp:Label ID="AddressViewTxCountLabel" runat="server" /></p>
        <asp:ListView ID="AddressViewListView" runat="server">
            <LayoutTemplate>
                <table runat="server" id="tblDepartments" width="640px" style="padding: 6px;">
                    <tr>
                        <th style="padding: 6px;">Time UTC</th>
                        <th style="padding: 6px;">Block</th>
                        <th style="padding: 6px;">TxId</th>
                        <th style="padding: 6px;">SCC</th>
                    </tr>
                    <tr runat="server" id="itemPlaceholder" />
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server">
                    <td style="padding: 6px;">
                        <%#Eval("time") %>
                    </td>
                    <td style="padding: 6px;">
                        <a href="/Default?block=<%#Eval("block_height")%>"><%#Eval("block_height")%></a>
                    </td>
                    <td style="padding: 6px;">
                        <a href="/Default?tx=<%#Eval("tx_id")%>"><%#Eval("tx_id")%></a>
                    </td>
                    <td style="padding: 6px;">
                        <%#Eval("value") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>

    </asp:Panel>
</asp:Content>
