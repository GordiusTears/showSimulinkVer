using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            MessageBox.Show("Simulinkモデルファイル（.slx）を指定してください。");
            return;
        }

        string slxPath = args[0];
        if (!File.Exists(slxPath))
        {
            MessageBox.Show("ファイルが見つかりません: " + slxPath);
            return;
        }

        try
        {
            using var archive = ZipFile.OpenRead(slxPath);
            var entry = archive.GetEntry("metadata/coreProperties.xml");
            if (entry == null)
            {
                MessageBox.Show("coreProperties.xml が見つかりません。");
                return;
            }

            using var stream = entry.Open();
            var doc = XDocument.Load(stream);

            // Open Packaging Conventions / Dublin Core の名前空間
            XNamespace cp = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
            XNamespace dc = "http://purl.org/dc/elements/1.1/";
            XNamespace dcterms = "http://purl.org/dc/terms/";

            // 値取り出し（存在しない場合は null）
            string matlabVersion = FirstValue(doc, cp + "version")
                                    ?? FirstValue(doc, dc + "creator")     // バージョンが creator に入る場合もある
                                    ?? "(不明)";
            string lastModifiedBy = FirstValue(doc, cp + "lastModifiedBy") ?? "(不明)";

            // created / modified は属性が付いていても Value で中身だけ取れる
            string createdRaw = FirstValue(doc, dcterms + "created") ?? "(不明)";
            string modifiedRaw = FirstValue(doc, dcterms + "modified") ?? "(不明)";

            // ISO 8601 を DateTimeOffset にパース（失敗したらそのまま表示）
            string createdDisp = TryAsLocalTime(createdRaw);
            string modifiedDisp = TryAsLocalTime(modifiedRaw);

            string message =
                $"MATLAB Version: {matlabVersion}\n" +
                $"作成日時: {createdDisp}\n" +
                $"更新日時: {modifiedDisp}\n" +
                $"最終編集者: {lastModifiedBy}";

            MessageBox.Show(message, "Simulinkモデル情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("エラー: " + ex.Message);
        }
    }

    // 最初の一致要素の文字列値を返す
    static string? FirstValue(XDocument doc, XName name) =>
        doc.Descendants(name).FirstOrDefault()?.Value;

    // ISO8601(Z含む) をローカル時刻で見やすく整形
    static string TryAsLocalTime(string s)
    {
        if (DateTimeOffset.TryParse(s, out var dto))
        {
            var local = dto.ToLocalTime(); // 実行マシンのローカルタイム
            return local.ToString("yyyy-MM-dd HH:mm:ss (zzz)");
        }
        return s;
    }
}
