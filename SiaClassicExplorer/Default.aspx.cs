using ClassLibrary;
using System;
using System.Data;
using System.Web.UI;

namespace SiaClassicExplorer
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                NothingFoundPanel.Visible = !TryView();
            }
            catch(Exception x)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = x.Message;
            }
        }

        private bool TryView()
        {
            if (Request.TryGetParam("search", out string s))
                return TryViewBlock(s) || ViewTx(s) || ViewAddress(s);

            if (Request.TryGetParam("block", out s))
                return TryViewBlock(s);

            if (Request.TryGetParam("tx", out s))
                return ViewTx(s);

            if (Request.TryGetParam("addr", out s))
                return ViewAddress(s);

            ViewBlockList(0);
            return true;
        }

        private bool ViewAddress(string addr)
        {
            DataTable dt;
            object nrecords;
            using (var sqlc = new MySqlClient(SiaClassicLib.Config.ConnectionString))
            {
                dt = sqlc.FillTable($"SELECT timestamp,block_height,tx_id,value FROM siacoinoutputs JOIN blocks on height=block_height " +
                    $"WHERE unlockhash ='{addr}' ORDER by height desc LIMIT 21");
                nrecords = sqlc.ExecuteScalar($"SELECT count(*) from siacoinoutputs WHERE unlockhash ='{addr}'");
            }
            if (dt.Rows.Count == 0)
                return false;

            dt.AddTimeColumn("time", "timestamp");

            string count = "20", last = "Showing last ", s = "s";
            if (dt.Rows.Count <= 20)
            {
                count = dt.Rows.Count.ToString();
                last = string.Empty;
                if (dt.Rows.Count == 1)
                    s = string.Empty;
            }

            AddressViewPanel.Visible = true;
            AddressViewListView.DataSource = dt;
            AddressViewListView.DataBind();
            AddressViewLabel.Text = addr;
            AddressViewTxCountLabel.Text = $"{last}{count} transaction{s} out of {nrecords}";
            return true;
        }

        private bool ViewTx(string txid)
        {
            DataTable dt, dttx;
            using (var sqlc = new MySqlClient(SiaClassicLib.Config.ConnectionString))
            {
                dt = sqlc.FillTable($"SELECT value,unlockhash FROM siacoinoutputs WHERE tx_id='{txid}'");
                dttx = sqlc.FillTable($"SELECT height,timestamp FROM txs JOIN blocks on height=blockheight WHERE txs.id='{txid}'");
            }
            if (dttx.Rows.Count == 0)
                return false;

            dttx.AddTimeColumn("time", "timestamp");
            var height = dttx.Rows[0]["height"].ToString();

            TxViewPanel.Visible = true;
            TxViewListView.DataSource = dt;
            TxViewListView.DataBind();
            TxViewIdLabel.Text = txid;
            TxViewHeightHyperLink.NavigateUrl = $"/Default?block={height}";
            TxViewHeightHyperLink.Text = "Block " + height;
            TxViewTimeLabel.Text = dttx.Rows[0]["time"].ToString();
            return true;
        }

        private bool TryViewBlock(string height)
        {
            if (!int.TryParse(height, out int h))
                return false;
            return h > 0 ? ViewBlock(h) : ViewBlockList(-h);
        }

        private bool ViewBlock(int height)
        {
            DataTable dt;
            string ts;
            using (var sqlc = new MySqlClient(SiaClassicLib.Config.ConnectionString))
            {
                dt = sqlc.FillTable($"SELECT id,siacoininputs,siacoinoutputs,filecontracts,filecontractrevisions,storageproofs,"
                    + "siafundinputs,siafundoutputs,minerfees,arbitrarydata,transactionsignatures FROM txs WHERE blockheight=" + height);
                ts = sqlc.ExecuteScalar($"SELECT timestamp FROM blocks WHERE height=" + height)?.ToString();
            }
            if (dt.Rows.Count == 0)
                return false;

            BlockViewPanel.Visible = true;
            BlockViewListView.DataSource = dt;
            BlockViewListView.DataBind();
            BlockHeightLabel.Text = height.ToString();
            BlockTimeLabel.Text = ClassLibrary.Extensions.TimeStampToString(ts);
            return true;
        }

        private bool ViewBlockList(int offset)
        {
            DataTable dt;
            using (var sqlc = new MySqlClient(SiaClassicLib.Config.ConnectionString))
            {
                dt = sqlc.FillTable($"SELECT height,timestamp,minerpayouts,tx_count FROM blocks ORDER BY height DESC LIMIT {offset},20");
            }
            if (dt.Rows.Count == 0)
                return false;

            dt.AddTimeColumn("time", "timestamp");

            BlockListPanel.Visible = true;
            BlockListView.DataSource = dt;
            BlockListView.DataBind();
            BlockListOffsetLabel.Text = offset.ToString();
            return true;
        }
    }
}