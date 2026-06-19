using System;
using System.Data;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace HastaneBilgiSistemi.Data
{
    internal static class Db
    {
        public static readonly string DatabasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HastaneBilgiSistemi",
            "hastane.db");

        private static readonly string ConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            ForeignKeys = true
        }.ToString();

        public static void Initialize()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath));
            Execute(SchemaSql);

            var count = Convert.ToInt32(Scalar("SELECT COUNT(*) FROM brans"));
            if (count == 0)
                Seed();
        }

        public static DataTable Query(string sql, params SqliteParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddRange(parameters);
                using (var reader = command.ExecuteReader())
                {
                    var table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        public static int Execute(string sql, params SqliteParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var transaction = connection.BeginTransaction())
            using (var command = new SqliteCommand(sql, connection, transaction))
            {
                command.Parameters.AddRange(parameters);
                var affected = 0;
                foreach (var statement in SplitSql(sql))
                {
                    command.CommandText = statement;
                    affected += command.ExecuteNonQuery();
                }

                transaction.Commit();
                return affected;
            }
        }

        public static object Scalar(string sql, params SqliteParameter[] parameters)
        {
            using (var connection = OpenConnection())
            using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteScalar();
            }
        }

        public static SqliteParameter P(string name, object value)
        {
            if (value is DateTime date)
                value = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            else if (value is TimeSpan time)
                value = time.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
            else if (value is bool boolean)
                value = boolean ? 1 : 0;

            return new SqliteParameter(name, value ?? DBNull.Value);
        }

        private static SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = ON;";
                command.ExecuteNonQuery();
            }

            return connection;
        }

        private static string[] SplitSql(string sql)
        {
            return sql.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void Seed()
        {
            using (var connection = OpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                ExecuteSeed(connection, transaction);
                transaction.Commit();
            }
        }

        private static void ExecuteSeed(SqliteConnection connection, SqliteTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;

                for (var i = 1; i <= 100; i++)
                {
                    command.CommandText = "INSERT INTO brans(ad, poliklinik_no) VALUES($ad, $poliklinik)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$ad", "Brans " + i.ToString("000"));
                    command.Parameters.AddWithValue("$poliklinik", "P" + i.ToString("000"));
                    command.ExecuteNonQuery();
                }

                for (var i = 1; i <= 100; i++)
                {
                    command.CommandText = "INSERT INTO doktor(ad, soyad, unvan, brans_id, calisma_gunleri, baslangic_saati, bitis_saati, aktif_mi) " +
                                          "VALUES($ad, $soyad, $unvan, $brans, $gunler, $baslangic, $bitis, $aktif)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$ad", "DoktorAd" + i);
                    command.Parameters.AddWithValue("$soyad", "DoktorSoyad" + i);
                    command.Parameters.AddWithValue("$unvan", i % 5 == 0 ? "Prof. Dr." : i % 3 == 0 ? "Doc. Dr." : "Uzm. Dr.");
                    command.Parameters.AddWithValue("$brans", i);
                    command.Parameters.AddWithValue("$gunler", i % 2 == 0 ? "Pazartesi,Carsamba,Cuma" : "Sali,Persembe");
                    command.Parameters.AddWithValue("$baslangic", "09:00");
                    command.Parameters.AddWithValue("$bitis", "17:00");
                    command.Parameters.AddWithValue("$aktif", i % 20 == 0 ? 0 : 1);
                    command.ExecuteNonQuery();
                }

                for (var i = 1; i <= 100; i++)
                {
                    command.CommandText = "INSERT INTO hasta(ad, soyad, tc_kimlik, dogum_tarihi, cinsiyet, telefon, adres, kan_grubu) " +
                                          "VALUES($ad, $soyad, $tc, $dogum, $cinsiyet, $telefon, $adres, $kan)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$ad", "HastaAd" + i);
                    command.Parameters.AddWithValue("$soyad", "HastaSoyad" + i);
                    command.Parameters.AddWithValue("$tc", (10000000000L + i).ToString(CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("$dogum", new DateTime(2010, 1, 1).AddDays(-(i * 120)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("$cinsiyet", i % 2 == 0 ? "K" : "E");
                    command.Parameters.AddWithValue("$telefon", "05" + (i * 379).ToString("000000000"));
                    command.Parameters.AddWithValue("$adres", "Mahalle " + i + ", Sokak " + ((i % 25) + 1));
                    command.Parameters.AddWithValue("$kan", BloodGroup(i));
                    command.ExecuteNonQuery();
                }

                for (var i = 1; i <= 150; i++)
                {
                    command.CommandText = "INSERT INTO randevu(hasta_id, doktor_id, tarih, saat, durum, sikayet) " +
                                          "VALUES($hasta, $doktor, $tarih, $saat, $durum, $sikayet)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$hasta", ((i - 1) % 100) + 1);
                    command.Parameters.AddWithValue("$doktor", ((i - 1) % 100) + 1);
                    command.Parameters.AddWithValue("$tarih", new DateTime(2026, 6, 1).AddDays(i % 45).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("$saat", new TimeSpan(9, 0, 0).Add(TimeSpan.FromMinutes(30 * (i % 16))).ToString(@"hh\:mm", CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("$durum", i % 10 == 0 ? "Iptal" : i % 3 == 0 ? "Tamamlandi" : "Bekliyor");
                    command.Parameters.AddWithValue("$sikayet", "Ornek sikayet " + i);
                    command.ExecuteNonQuery();
                }

                for (var i = 1; i <= 100; i++)
                {
                    command.CommandText = "INSERT INTO recete(hasta_id, doktor_id, randevu_id, tarih, teshis, notlar) " +
                                          "SELECT hasta_id, doktor_id, randevu_id, tarih, $teshis, $notlar FROM randevu WHERE randevu_id=$randevu";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$teshis", "Teshis " + i);
                    command.Parameters.AddWithValue("$notlar", "Doktor notu " + i);
                    command.Parameters.AddWithValue("$randevu", i);
                    command.ExecuteNonQuery();
                }

                for (var i = 1; i <= 200; i++)
                {
                    command.CommandText = "INSERT INTO recete_kalem(recete_id, ilac_adi, doz, kullanim_sekli, sure_gun) " +
                                          "VALUES($recete, $ilac, $doz, $kullanim, $sure)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$recete", ((i - 1) % 100) + 1);
                    command.Parameters.AddWithValue("$ilac", "Ilac " + (((i - 1) % 30) + 1));
                    command.Parameters.AddWithValue("$doz", i % 2 == 0 ? "500mg" : "250mg");
                    command.Parameters.AddWithValue("$kullanim", i % 3 == 0 ? "Gunde 1x1" : "Gunde 2x1 yemekten sonra");
                    command.Parameters.AddWithValue("$sure", (i % 14) + 1);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string BloodGroup(int i)
        {
            switch (i % 8)
            {
                case 0: return "A+";
                case 1: return "A-";
                case 2: return "B+";
                case 3: return "B-";
                case 4: return "AB+";
                case 5: return "AB-";
                case 6: return "0+";
                default: return "0-";
            }
        }

        private const string SchemaSql = @"
CREATE TABLE IF NOT EXISTS brans
(
    brans_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ad TEXT NOT NULL UNIQUE,
    poliklinik_no TEXT
);

CREATE TABLE IF NOT EXISTS doktor
(
    doktor_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ad TEXT NOT NULL,
    soyad TEXT NOT NULL,
    unvan TEXT,
    brans_id INTEGER NOT NULL,
    calisma_gunleri TEXT,
    baslangic_saati TEXT NOT NULL,
    bitis_saati TEXT NOT NULL,
    aktif_mi INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (brans_id) REFERENCES brans(brans_id),
    CHECK (baslangic_saati < bitis_saati),
    CHECK (aktif_mi IN (0, 1))
);

CREATE TABLE IF NOT EXISTS hasta
(
    hasta_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ad TEXT NOT NULL,
    soyad TEXT NOT NULL,
    tc_kimlik TEXT NOT NULL UNIQUE,
    dogum_tarihi TEXT NOT NULL,
    cinsiyet TEXT NOT NULL,
    telefon TEXT,
    adres TEXT,
    kan_grubu TEXT,
    kayit_tarihi TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (cinsiyet IN ('E', 'K')),
    CHECK (length(tc_kimlik) = 11 AND tc_kimlik NOT GLOB '*[^0-9]*')
);

CREATE TABLE IF NOT EXISTS randevu
(
    randevu_id INTEGER PRIMARY KEY AUTOINCREMENT,
    hasta_id INTEGER NOT NULL,
    doktor_id INTEGER NOT NULL,
    tarih TEXT NOT NULL,
    saat TEXT NOT NULL,
    durum TEXT NOT NULL DEFAULT 'Bekliyor',
    sikayet TEXT,
    olusturma_zamani TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (hasta_id) REFERENCES hasta(hasta_id),
    FOREIGN KEY (doktor_id) REFERENCES doktor(doktor_id),
    CHECK (durum IN ('Bekliyor', 'Tamamlandi', 'Iptal'))
);

CREATE UNIQUE INDEX IF NOT EXISTS UX_randevu_doktor_tarih_saat
ON randevu(doktor_id, tarih, saat)
WHERE durum <> 'Iptal';

CREATE TABLE IF NOT EXISTS recete
(
    recete_id INTEGER PRIMARY KEY AUTOINCREMENT,
    hasta_id INTEGER NOT NULL,
    doktor_id INTEGER NOT NULL,
    randevu_id INTEGER NOT NULL UNIQUE,
    tarih TEXT NOT NULL DEFAULT CURRENT_DATE,
    teshis TEXT,
    notlar TEXT,
    FOREIGN KEY (hasta_id) REFERENCES hasta(hasta_id),
    FOREIGN KEY (doktor_id) REFERENCES doktor(doktor_id),
    FOREIGN KEY (randevu_id) REFERENCES randevu(randevu_id)
);

CREATE TABLE IF NOT EXISTS recete_kalem
(
    kalem_id INTEGER PRIMARY KEY AUTOINCREMENT,
    recete_id INTEGER NOT NULL,
    ilac_adi TEXT NOT NULL,
    doz TEXT NOT NULL,
    kullanim_sekli TEXT,
    sure_gun INTEGER NOT NULL,
    FOREIGN KEY (recete_id) REFERENCES recete(recete_id) ON DELETE CASCADE,
    CHECK (sure_gun > 0)
);

CREATE VIEW IF NOT EXISTS vw_BransAylikRandevuSayisi
AS
SELECT
    b.brans_id,
    b.ad AS brans,
    CAST(strftime('%Y', r.tarih) AS INTEGER) AS yil,
    CAST(strftime('%m', r.tarih) AS INTEGER) AS ay,
    COUNT(*) AS randevu_sayisi,
    SUM(CASE WHEN r.durum = 'Tamamlandi' THEN 1 ELSE 0 END) AS tamamlanan_sayisi,
    SUM(CASE WHEN r.durum = 'Iptal' THEN 1 ELSE 0 END) AS iptal_sayisi
FROM brans b
INNER JOIN doktor d ON d.brans_id = b.brans_id
INNER JOIN randevu r ON r.doktor_id = d.doktor_id
GROUP BY b.brans_id, b.ad, strftime('%Y', r.tarih), strftime('%m', r.tarih);

CREATE VIEW IF NOT EXISTS vw_HastaGecmis
AS
SELECT
    h.hasta_id,
    h.ad || ' ' || h.soyad AS hasta,
    r.tarih,
    r.saat,
    d.unvan || ' ' || d.ad || ' ' || d.soyad AS doktor,
    b.ad AS brans,
    r.durum,
    r.sikayet,
    re.recete_id,
    re.teshis
FROM hasta h
LEFT JOIN randevu r ON r.hasta_id = h.hasta_id
LEFT JOIN doktor d ON d.doktor_id = r.doktor_id
LEFT JOIN brans b ON b.brans_id = d.brans_id
LEFT JOIN recete re ON re.randevu_id = r.randevu_id;
";
    }
}
