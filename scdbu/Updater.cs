using ClassLibrary;
using SiaClassicLib;
using System;
using System.Threading;

namespace scdbu
{
    class Updater
    {
        readonly WebClient wc;
        readonly string connStr;
        public Updater(string uriBase, string connStr)
        {
            wc = new WebClient(uriBase, "Sia-Agent");
            this.connStr = connStr;
        }

        public void UpdateTopOnly(int heightLimit, bool continuos)
        {
            do
                try
                {
                    int height;
                    using (var sqlc = new MySqlClient(connStr))
                    {
                        var s = sqlc.ExecuteScalar($"SELECT height FROM `blocks` ORDER BY height DESC LIMIT 1");
                        height = s != null ? (int)(uint)s + 1 : 0;
                    }
                    Console.WriteLine(DateTime.Now + " Db height=" + height);

                    if (height < heightLimit)
                        height = heightLimit;

                    int consensus_height = -1;
                    bool wasSleeping = false;
                    while (true)
                    {
                        if (Console.KeyAvailable && Console.ReadKey().KeyChar == 27)
                            break;

                        dynamic consensus = wc.GetObject("consensus");
                        int new_consensus_height = (int)consensus.height;

                        if (consensus_height == new_consensus_height)
                        {
                            if (continuos) { Thread.Sleep(10000); Console.Write("."); wasSleeping = true; continue; }
                            else break;
                        }
                        if (wasSleeping) { Console.WriteLine(); wasSleeping = false; }

                        Console.WriteLine(DateTime.Now + " Consensus is at height=" + new_consensus_height);
                        consensus_height = new_consensus_height;

                        using (var sqlc = new MySqlClient(connStr))
                            for (; height <= consensus_height; ++height)
                            {
                                if (Console.KeyAvailable && Console.ReadKey().KeyChar == 27)
                                    break;
                                InsertAtHeight(sqlc, height);
                            }
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                    Console.WriteLine(x.StackTrace);
                }
            while (continuos);
        }

        //public int Update(int heightLimit, bool topOnly)
        //{
        //    dynamic consensus = wc.GetObject("consensus");
        //    Console.WriteLine(DateTime.Now + " Consensus is at height=" + consensus.height);

        //    int height = -1;
        //    using (var sqlc = new MySqlClient(connStr))
        //    {
        //        int limit = topOnly ? 1 : 1000;
        //        DataTable dt = sqlc.FillTable($"SELECT height FROM `blocks` ORDER BY height DESC LIMIT {limit}");
        //        int dbheight = dt.Rows.Count > 0 ? (int)dt.Rows[0][0] + 1 : 0;

        //        if (dbheight < heightLimit)
        //            dbheight = heightLimit;

        //        // before existing db records
        //        for (height = consensus.height; height >= dbheight; --height)
        //        {
        //            InsertAtHeight(sqlc, height);
        //        }
        //        if (topOnly)
        //            return height;

        //        // through existing db records
        //        int topOffset = 0;
        //        int dbidx = 0;
        //        while (height >= heightLimit)
        //        {
        //            if (dt == null || dbidx > dt.Rows.Count)
        //            {
        //                dt = sqlc.FillTable($"SELECT height FROM `blocks` ORDER BY height DESC LIMIT {topOffset},1000");
        //                if (dt.Rows.Count == 0)
        //                    break; // nothing more in db
        //                dbidx = 0;
        //                topOffset += 1000;
        //            }
        //            if (height == (int)dt.Rows[dbidx][0])
        //                ++dbidx;
        //            else
        //            {
        //                InsertAtHeight(sqlc, height--);
        //                ++topOffset;
        //            }
        //        }

        //        // past existing db records
        //        for (; height >= dbheight; --height)
        //        {
        //            InsertAtHeight(sqlc, height);
        //        }

        //        //    for (height = consensus.height; height >= heightLimit; height--)
        //        //    {
        //        //        if (checkDb)
        //        //        {
        //        //            if (heights.Count == 0)
        //        //            {
        //        //                Console.WriteLine(DateTime.Now + " Checking heights saved in db");
        //        //                var dt = sqlc.FillTable($"SELECT height FROM `blocks` ORDER BY height DESC LIMIT {topOffset},1000");
        //        //                if (heights.Count == 0)
        //        //                {
        //        //                    checkDb = false; // nothing more in db
        //        //                    Console.WriteLine("Nothing more in db");
        //        //                    goto nothing_more_in_db;
        //        //                }
        //        //            }

        //        //            ++topOffset;
        //        //            Debug.Assert(heights.ElementAt(0) <= height);
        //        //            if (heights.ElementAt(0) == height)
        //        //            {
        //        //                if (lastOnly)
        //        //                    break;
        //        //                heights.RemoveAt(0);
        //        //                continue;
        //        //            }
        //        //        }

        //        //    nothing_more_in_db:
        //        //        UpdateAtHeight(sqlc, height);

        //        //        if (Console.KeyAvailable && Console.ReadKey().KeyChar == 3)
        //        //            break;
        //        //    }
        //    }
        //    return height;
        //}


        public string GetBlockJson(int height)
        {
            return wc.Get("consensus/blocks?height=" + height);
        }

        private void InsertAtHeight(MySqlClient sqlc, int height)
        {
            Console.WriteLine(DateTime.Now + " Writing block height=" + height);

            dynamic b = wc.GetObject("consensus/blocks?height=" + height);

            var minerpayouts = "0";
            using (var h = new Hastings())
            {
                if (b.minerpayouts != null)
                    foreach (var v in b.minerpayouts)
                        h.Add(v.value.Value);
                minerpayouts = h.ToString();
            }

            var sql = $"INSERT INTO `blocks` VALUES('{b.height}', '{b.id}', '{b.parentid}', '{b.timestamp}', " +
                $"'{minerpayouts}', '{b.transactions.Count}');\n"
                ;

            foreach (dynamic tx in b.transactions)
            {
                sql += $"INSERT INTO `txs` VALUES('{tx.id}', '{b.height}', '{tx.siacoininputs.Count}','{tx.siacoinoutputs.Count}'," +
                    $" '{tx.filecontracts.Count}','{tx.filecontractrevisions.Count}','{tx.storageproofs.Count}','{tx.siafundinputs.Count}'," +
                    $" '{tx.siafundoutputs.Count}','{tx.minerfees.Count}','{tx.arbitrarydata.Count}','{tx.transactionsignatures.Count}');\n";

                foreach (dynamic output in tx.siacoinoutputs)
                {
                    using (var h = new Hastings())
                    {
                        h.Add(output.value.Value);
                        sql += $"INSERT INTO `siacoinoutputs` VALUES('{output.id}', '{b.height}', '{tx.id}', '{h.ToString()}', '{output.unlockhash}');\n";
                    }
                }
            }

            //sqlc.ExecuteNonQuery($"START TRANSACTION;\n{sql}COMMIT;");
            sqlc.ExecuteNonQuery(sql);
        }

        private void UpdateAtHeight(MySqlClient sqlc, int height)
        {
            Console.WriteLine(DateTime.Now + " Updating at height=" + height);

            dynamic b = wc.GetObject("consensus/blocks?height=" + height);

            // ONLY outputs NOW 
            string sql = string.Empty;

            //var minerpayouts = "0";
            //using (var h = new Hastings())
            //{
            //    if (b.minerpayouts != null)
            //        foreach (var v in b.minerpayouts)
            //            h.Add(v.value.Value);
            //    minerpayouts = h.ToString();
            //}

            //var sql = $"INSERT INTO `blocks` VALUES('{b.height}', '{b.id}', '{b.parentid}', '{b.timestamp}', " +
            //    $"'{minerpayouts}', '{b.transactions.Count}');\n"
            //    ;

            foreach (dynamic tx in b.transactions)
            {
                //sql += $"INSERT INTO `txs` VALUES('{tx.id}', '{b.height}', '{tx.siacoininputs.Count}','{tx.siacoinoutputs.Count}'," +
                //    $" '{tx.filecontracts.Count}','{tx.filecontractrevisions.Count}','{tx.storageproofs.Count}','{tx.siafundinputs.Count}'," +
                //    $" '{tx.siafundoutputs.Count}','{tx.minerfees.Count}','{tx.arbitrarydata.Count}','{tx.transactionsignatures.Count}');\n";

                foreach (dynamic output in tx.siacoinoutputs)
                {
                    //using (var h = new Hastings())
                    //{
                    //    h.Add(output.value.Value);
                    //    sql += $"INSERT INTO `siacoinoutputs` VALUES('{output.id}', '{b.height}', '{tx.id}', '{h.ToString()}', '{output.unlockhash}');\n";
                    //}
                    sql += $"UPDATE siacoinoutputs SET unlockhash='{output.unlockhash}' WHERE id='{output.id}';\n";
                }
            }

            //sqlc.ExecuteNonQuery($"START TRANSACTION;\n{sql}COMMIT;");
            if (sql.Length > 0)
                sqlc.ExecuteNonQuery(sql);
        }


        public void UpdateOutput(int heightStart = 180000, int heightEnd = 212578)
        {
            try
            {
                using (var sqlc = new MySqlClient(connStr))
                    for (int height = heightStart; height <= heightEnd; ++height)
                    {
                        if (Console.KeyAvailable && Console.ReadKey().KeyChar == 27)
                            break;
                        UpdateAtHeight(sqlc, height);
                    }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine(x.StackTrace);
            }
        }
    }
}
