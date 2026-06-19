using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using HastaneBilgiSistemi.Data;
using Microsoft.Data.Sqlite;

namespace HastaneBilgiSistemi.Forms
{
    public class MainForm : Form
    {
        private readonly TabControl tabs = new TabControl();

        public MainForm()
        {
            Text = "Hastane Bilgi Yonetim Sistemi";
            Width = 1280;
            Height = 820;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);

            tabs.Dock = DockStyle.Fill;
            Controls.Add(tabs);

            BuildBransTab();
            BuildDoktorTab();
            BuildHastaTab();
            BuildRandevuTab();
            BuildReceteTab();
            BuildReceteKalemTab();
            BuildHastaGecmisiTab();
            BuildRaporTab();
        }

        private void BuildBransTab()
        {
            var grid = Grid();
            var form = Fields(new[] { "ad", "poliklinik_no" });
            var tab = CrudTab("Brans", grid, form);
            tabs.TabPages.Add(tab);

            Action load = delegate
            {
                grid.DataSource = Db.Query("SELECT brans_id, ad, poliklinik_no FROM brans ORDER BY brans_id");
            };
            grid.SelectionChanged += delegate { Fill(form, grid); };

            AddCrudButtons(tab, load,
                delegate
                {
                    Db.Execute("INSERT INTO brans(ad, poliklinik_no) VALUES(@ad, @poliklinik_no)",
                        Db.P("@ad", V(form, "ad")), Db.P("@poliklinik_no", V(form, "poliklinik_no")));
                },
                delegate
                {
                    Db.Execute("UPDATE brans SET ad=@ad, poliklinik_no=@poliklinik_no WHERE brans_id=@id",
                        Db.P("@ad", V(form, "ad")), Db.P("@poliklinik_no", V(form, "poliklinik_no")), Db.P("@id", Id(grid, "brans_id")));
                },
                delegate
                {
                    Db.Execute("DELETE FROM brans WHERE brans_id=@id", Db.P("@id", Id(grid, "brans_id")));
                });
            Safe(load);
        }

        private void BuildDoktorTab()
        {
            var grid = Grid();
            var form = Fields(new[]
            {
                "ad", "soyad", "unvan", "brans_id", "calisma_gunleri", "baslangic_saati", "bitis_saati", "aktif_mi"
            });
            var tab = CrudTab("Doktor", grid, form);
            tabs.TabPages.Add(tab);

            Action load = delegate
            {
                grid.DataSource = Db.Query(
                    "SELECT d.doktor_id, d.ad, d.soyad, d.unvan, d.brans_id, b.ad AS brans, d.calisma_gunleri, " +
                    "d.baslangic_saati, d.bitis_saati, d.aktif_mi FROM doktor d " +
                    "LEFT JOIN brans b ON b.brans_id=d.brans_id ORDER BY d.doktor_id");
            };
            grid.SelectionChanged += delegate { Fill(form, grid); };

            AddCrudButtons(tab, load,
                delegate
                {
                    Db.Execute("INSERT INTO doktor(ad, soyad, unvan, brans_id, calisma_gunleri, baslangic_saati, bitis_saati, aktif_mi) " +
                               "VALUES(@ad, @soyad, @unvan, @brans_id, @calisma_gunleri, @baslangic_saati, @bitis_saati, @aktif_mi)",
                        Db.P("@ad", V(form, "ad")), Db.P("@soyad", V(form, "soyad")), Db.P("@unvan", V(form, "unvan")),
                        Db.P("@brans_id", IntV(form, "brans_id")), Db.P("@calisma_gunleri", V(form, "calisma_gunleri")),
                        Db.P("@baslangic_saati", TimeV(form, "baslangic_saati")), Db.P("@bitis_saati", TimeV(form, "bitis_saati")),
                        Db.P("@aktif_mi", BoolV(form, "aktif_mi")));
                },
                delegate
                {
                    Db.Execute("UPDATE doktor SET ad=@ad, soyad=@soyad, unvan=@unvan, brans_id=@brans_id, calisma_gunleri=@calisma_gunleri, " +
                               "baslangic_saati=@baslangic_saati, bitis_saati=@bitis_saati, aktif_mi=@aktif_mi WHERE doktor_id=@id",
                        Db.P("@ad", V(form, "ad")), Db.P("@soyad", V(form, "soyad")), Db.P("@unvan", V(form, "unvan")),
                        Db.P("@brans_id", IntV(form, "brans_id")), Db.P("@calisma_gunleri", V(form, "calisma_gunleri")),
                        Db.P("@baslangic_saati", TimeV(form, "baslangic_saati")), Db.P("@bitis_saati", TimeV(form, "bitis_saati")),
                        Db.P("@aktif_mi", BoolV(form, "aktif_mi")), Db.P("@id", Id(grid, "doktor_id")));
                },
                delegate
                {
                    Db.Execute("DELETE FROM doktor WHERE doktor_id=@id", Db.P("@id", Id(grid, "doktor_id")));
                });
            Safe(load);
        }

        private void BuildHastaTab()
        {
            var grid = Grid();
            var form = Fields(new[]
            {
                "ad", "soyad", "tc_kimlik", "dogum_tarihi", "cinsiyet", "telefon", "adres", "kan_grubu"
            });
            var tab = CrudTab("Hasta", grid, form);
            tabs.TabPages.Add(tab);

            Action load = delegate
            {
                grid.DataSource = Db.Query("SELECT hasta_id, ad, soyad, tc_kimlik, dogum_tarihi, cinsiyet, telefon, adres, kan_grubu, kayit_tarihi FROM hasta ORDER BY hasta_id");
            };
            grid.SelectionChanged += delegate { Fill(form, grid); };

            AddCrudButtons(tab, load,
                delegate
                {
                    Db.Execute("INSERT INTO hasta(ad, soyad, tc_kimlik, dogum_tarihi, cinsiyet, telefon, adres, kan_grubu) " +
                               "VALUES(@ad, @soyad, @tc, @dogum, @cinsiyet, @telefon, @adres, @kan)",
                        Db.P("@ad", V(form, "ad")), Db.P("@soyad", V(form, "soyad")), Db.P("@tc", V(form, "tc_kimlik")),
                        Db.P("@dogum", DateV(form, "dogum_tarihi")), Db.P("@cinsiyet", V(form, "cinsiyet")),
                        Db.P("@telefon", V(form, "telefon")), Db.P("@adres", V(form, "adres")), Db.P("@kan", V(form, "kan_grubu")));
                },
                delegate
                {
                    Db.Execute("UPDATE hasta SET ad=@ad, soyad=@soyad, tc_kimlik=@tc, dogum_tarihi=@dogum, cinsiyet=@cinsiyet, " +
                               "telefon=@telefon, adres=@adres, kan_grubu=@kan WHERE hasta_id=@id",
                        Db.P("@ad", V(form, "ad")), Db.P("@soyad", V(form, "soyad")), Db.P("@tc", V(form, "tc_kimlik")),
                        Db.P("@dogum", DateV(form, "dogum_tarihi")), Db.P("@cinsiyet", V(form, "cinsiyet")),
                        Db.P("@telefon", V(form, "telefon")), Db.P("@adres", V(form, "adres")), Db.P("@kan", V(form, "kan_grubu")),
                        Db.P("@id", Id(grid, "hasta_id")));
                },
                delegate
                {
                    Db.Execute("DELETE FROM hasta WHERE hasta_id=@id", Db.P("@id", Id(grid, "hasta_id")));
                });
            Safe(load);
        }

        private void BuildRandevuTab()
        {
            var grid = Grid();
            var form = Fields(new[] { "hasta_id", "doktor_id", "tarih", "saat", "durum", "sikayet" });
            var tab = CrudTab("Randevu", grid, form);
            var available = new ListBox { Width = 220, Height = 150 };
            var rightPanel = tab.Controls["rightPanel"] as Panel;
            var slotsPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(0, 12, 0, 0) };
            slotsPanel.Controls.Add(Header("Musait Saatler"));
            slotsPanel.Controls.Add(available);
            rightPanel.Controls.Add(slotsPanel);
            tabs.TabPages.Add(tab);

            Action load = delegate
            {
                grid.DataSource = Db.Query(
                    "SELECT r.randevu_id, r.hasta_id, h.ad || ' ' || h.soyad AS hasta, r.doktor_id, d.ad || ' ' || d.soyad AS doktor, " +
                    "r.tarih, r.saat, r.durum, r.sikayet, r.olusturma_zamani FROM randevu r " +
                    "INNER JOIN hasta h ON h.hasta_id=r.hasta_id INNER JOIN doktor d ON d.doktor_id=r.doktor_id ORDER BY r.tarih DESC, r.saat");
            };
            grid.SelectionChanged += delegate { Fill(form, grid); };

            AddCrudButtons(tab, load,
                delegate
                {
                    EnsureActiveDoctor(IntV(form, "doktor_id"));
                    Db.Execute("INSERT INTO randevu(hasta_id, doktor_id, tarih, saat, durum, sikayet) " +
                               "VALUES(@hasta_id, @doktor_id, @tarih, @saat, @durum, @sikayet)",
                        Db.P("@hasta_id", IntV(form, "hasta_id")), Db.P("@doktor_id", IntV(form, "doktor_id")),
                        Db.P("@tarih", DateV(form, "tarih")), Db.P("@saat", TimeV(form, "saat")),
                        Db.P("@durum", V(form, "durum")), Db.P("@sikayet", V(form, "sikayet")));
                },
                delegate
                {
                    EnsureActiveDoctor(IntV(form, "doktor_id"));
                    Db.Execute("UPDATE randevu SET hasta_id=@hasta_id, doktor_id=@doktor_id, tarih=@tarih, saat=@saat, durum=@durum, sikayet=@sikayet " +
                               "WHERE randevu_id=@id",
                        Db.P("@hasta_id", IntV(form, "hasta_id")), Db.P("@doktor_id", IntV(form, "doktor_id")),
                        Db.P("@tarih", DateV(form, "tarih")), Db.P("@saat", TimeV(form, "saat")),
                        Db.P("@durum", V(form, "durum")), Db.P("@sikayet", V(form, "sikayet")), Db.P("@id", Id(grid, "randevu_id")));
                },
                delegate
                {
                    Db.Execute("DELETE FROM randevu WHERE randevu_id=@id", Db.P("@id", Id(grid, "randevu_id")));
                });

            var showSlots = Button("Musait Saatleri Goster");
            showSlots.Click += delegate
            {
                available.Items.Clear();
                var doctorId = IntV(form, "doktor_id");
                var date = DateV(form, "tarih");
                var taken = Db.Query("SELECT saat FROM randevu WHERE doktor_id=@doktor_id AND tarih=@tarih AND durum<>'Iptal'",
                    Db.P("@doktor_id", doctorId), Db.P("@tarih", date));
                var busy = new HashSet<string>();
                foreach (DataRow row in taken.Rows)
                    busy.Add(row["saat"].ToString());

                for (var time = new TimeSpan(9, 0, 0); time <= new TimeSpan(16, 30, 0); time = time.Add(TimeSpan.FromMinutes(30)))
                {
                    var text = time.ToString(@"hh\:mm");
                    if (!busy.Contains(text))
                        available.Items.Add(text);
                }
            };
            slotsPanel.Controls.Add(showSlots);
            available.DoubleClick += delegate
            {
                if (available.SelectedItem != null)
                    form["saat"].Text = available.SelectedItem.ToString();
            };
            Safe(load);
        }

        private void BuildReceteTab()
        {
            var grid = Grid();
            var form = Fields(new[] { "hasta_id", "doktor_id", "randevu_id", "tarih", "teshis", "notlar" });
            var tab = CrudTab("Recete", grid, form);
            tabs.TabPages.Add(tab);

            Action load = delegate
            {
                grid.DataSource = Db.Query(
                    "SELECT re.recete_id, re.hasta_id, h.ad || ' ' || h.soyad AS hasta, re.doktor_id, d.ad || ' ' || d.soyad AS doktor, " +
                    "re.randevu_id, re.tarih, re.teshis, re.notlar FROM recete re " +
                    "INNER JOIN hasta h ON h.hasta_id=re.hasta_id INNER JOIN doktor d ON d.doktor_id=re.doktor_id ORDER BY re.recete_id DESC");
            };
            grid.SelectionChanged += delegate { Fill(form, grid); };

            AddCrudButtons(tab, load,
                delegate
                {
                    Db.Execute("INSERT INTO recete(hasta_id, doktor_id, randevu_id, tarih, teshis, notlar) VALUES(@hasta_id, @doktor_id, @randevu_id, @tarih, @teshis, @notlar)",
                        Db.P("@hasta_id", IntV(form, "hasta_id")), Db.P("@doktor_id", IntV(form, "doktor_id")),
                        Db.P("@randevu_id", IntV(form, "randevu_id")), Db.P("@tarih", DateV(form, "tarih")),
                        Db.P("@teshis", V(form, "teshis")), Db.P("@notlar", V(form, "notlar")));
                },
                delegate
                {
                    Db.Execute("UPDATE recete SET hasta_id=@hasta_id, doktor_id=@doktor_id, randevu_id=@randevu_id, tarih=@tarih, teshis=@teshis, notlar=@notlar WHERE recete_id=@id",
                        Db.P("@hasta_id", IntV(form, "hasta_id")), Db.P("@doktor_id", IntV(form, "doktor_id")),
                        Db.P("@randevu_id", IntV(form, "randevu_id")), Db.P("@tarih", DateV(form, "tarih")),
                        Db.P("@teshis", V(form, "teshis")), Db.P("@notlar", V(form, "notlar")), Db.P("@id", Id(grid, "recete_id")));
                },
                delegate
                {
                    Db.Execute("DELETE FROM recete WHERE recete_id=@id", Db.P("@id", Id(grid, "recete_id")));
                });
            Safe(load);
        }

        private void BuildReceteKalemTab()
        {
            var grid = Grid();
            var form = Fields(new[] { "recete_id", "ilac_adi", "doz", "kullanim_sekli", "sure_gun" });
            var tab = CrudTab("Recete Kalem", grid, form);
            tabs.TabPages.Add(tab);

            Action load = delegate
            {
                grid.DataSource = Db.Query("SELECT kalem_id, recete_id, ilac_adi, doz, kullanim_sekli, sure_gun FROM recete_kalem ORDER BY kalem_id DESC");
            };
            grid.SelectionChanged += delegate { Fill(form, grid); };

            AddCrudButtons(tab, load,
                delegate
                {
                    Db.Execute("INSERT INTO recete_kalem(recete_id, ilac_adi, doz, kullanim_sekli, sure_gun) VALUES(@recete_id, @ilac, @doz, @kullanim, @sure)",
                        Db.P("@recete_id", IntV(form, "recete_id")), Db.P("@ilac", V(form, "ilac_adi")),
                        Db.P("@doz", V(form, "doz")), Db.P("@kullanim", V(form, "kullanim_sekli")), Db.P("@sure", IntV(form, "sure_gun")));
                },
                delegate
                {
                    Db.Execute("UPDATE recete_kalem SET recete_id=@recete_id, ilac_adi=@ilac, doz=@doz, kullanim_sekli=@kullanim, sure_gun=@sure WHERE kalem_id=@id",
                        Db.P("@recete_id", IntV(form, "recete_id")), Db.P("@ilac", V(form, "ilac_adi")),
                        Db.P("@doz", V(form, "doz")), Db.P("@kullanim", V(form, "kullanim_sekli")), Db.P("@sure", IntV(form, "sure_gun")),
                        Db.P("@id", Id(grid, "kalem_id")));
                },
                delegate
                {
                    Db.Execute("DELETE FROM recete_kalem WHERE kalem_id=@id", Db.P("@id", Id(grid, "kalem_id")));
                });
            Safe(load);
        }

        private void BuildHastaGecmisiTab()
        {
            var page = new TabPage("Hasta Gecmisi");
            var panel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight };
            var hastaId = new TextBox { Width = 100 };
            var grid = Grid();
            var button = Button("Gecmisi Getir");
            panel.Controls.Add(Header("Hasta ID"));
            panel.Controls.Add(hastaId);
            panel.Controls.Add(button);
            page.Controls.Add(grid);
            page.Controls.Add(panel);
            tabs.TabPages.Add(page);

            button.Click += delegate
            {
                grid.DataSource = Db.Query(
                    "SELECT 'Randevu' AS kayit_tipi, r.tarih AS tarih, r.saat AS saat, " +
                    "d.ad || ' ' || d.soyad AS doktor, r.durum AS detay, r.sikayet AS aciklama " +
                    "FROM randevu r INNER JOIN doktor d ON d.doktor_id=r.doktor_id WHERE r.hasta_id=@hasta_id " +
                    "UNION ALL SELECT 'Recete', re.tarih, '', d.ad || ' ' || d.soyad, re.teshis, re.notlar " +
                    "FROM recete re INNER JOIN doktor d ON d.doktor_id=re.doktor_id WHERE re.hasta_id=@hasta_id ORDER BY tarih DESC",
                    Db.P("@hasta_id", int.Parse(hastaId.Text)));
            };
        }

        private void BuildRaporTab()
        {
            var page = new TabPage("Raporlar");
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 64, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight };
            var grid = Grid();
            var start = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 115, Value = DateTime.Today.AddDays(-7) };
            var end = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 115, Value = DateTime.Today.AddDays(7) };
            var doctorReport = Button("Doktor Randevu Raporu");
            var branchReport = Button("Brans Aylik Yogunluk");
            var prescriptionReport = Button("Recete Ilac Dokumu");
            var noAppointmentReport = Button("Randevusuz Hastalar");

            top.Controls.Add(Header("Baslangic"));
            top.Controls.Add(start);
            top.Controls.Add(Header("Bitis"));
            top.Controls.Add(end);
            top.Controls.Add(doctorReport);
            top.Controls.Add(branchReport);
            top.Controls.Add(prescriptionReport);
            top.Controls.Add(noAppointmentReport);

            page.Controls.Add(grid);
            page.Controls.Add(top);
            tabs.TabPages.Add(page);

            doctorReport.Click += delegate
            {
                grid.DataSource = Db.Query(
                    "SELECT d.doktor_id, d.unvan || ' ' || d.ad || ' ' || d.soyad AS doktor, r.tarih, COUNT(*) AS randevu_sayisi, " +
                    "SUM(CASE WHEN r.durum='Tamamlandi' THEN 1 ELSE 0 END) AS tamamlanan, SUM(CASE WHEN r.durum='Iptal' THEN 1 ELSE 0 END) AS iptal " +
                    "FROM doktor d INNER JOIN randevu r ON r.doktor_id=d.doktor_id WHERE r.tarih BETWEEN @b AND @e " +
                    "GROUP BY d.doktor_id, d.unvan, d.ad, d.soyad, r.tarih ORDER BY r.tarih, doktor",
                    Db.P("@b", start.Value.Date), Db.P("@e", end.Value.Date));
            };

            branchReport.Click += delegate
            {
                grid.DataSource = Db.Query("SELECT * FROM vw_BransAylikRandevuSayisi ORDER BY yil DESC, ay DESC, randevu_sayisi DESC");
            };

            prescriptionReport.Click += delegate
            {
                grid.DataSource = Db.Query(
                    "SELECT h.hasta_id, h.ad || ' ' || h.soyad AS hasta, re.tarih, d.ad || ' ' || d.soyad AS doktor, " +
                    "re.teshis, rk.ilac_adi, rk.doz, rk.kullanim_sekli, rk.sure_gun FROM recete re " +
                    "INNER JOIN hasta h ON h.hasta_id=re.hasta_id INNER JOIN doktor d ON d.doktor_id=re.doktor_id " +
                    "LEFT JOIN recete_kalem rk ON rk.recete_id=re.recete_id WHERE re.tarih BETWEEN @b AND @e ORDER BY re.tarih DESC, hasta",
                    Db.P("@b", start.Value.Date), Db.P("@e", end.Value.Date));
            };

            noAppointmentReport.Click += delegate
            {
                grid.DataSource = Db.Query("SELECT hasta_id, ad, soyad, tc_kimlik, telefon FROM hasta WHERE hasta_id NOT IN (SELECT DISTINCT hasta_id FROM randevu) ORDER BY hasta_id");
            };
        }

        private static TabPage CrudTab(string title, DataGridView grid, Dictionary<string, TextBox> form)
        {
            var page = new TabPage(title);
            var right = new Panel { Name = "rightPanel", Dock = DockStyle.Right, Width = 310, Padding = new Padding(8), AutoScroll = true };
            var formPanel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            foreach (var item in form)
            {
                formPanel.Controls.Add(Header(item.Key), 0, formPanel.RowCount);
                formPanel.Controls.Add(item.Value, 1, formPanel.RowCount);
                formPanel.RowCount++;
            }

            right.Controls.Add(formPanel);
            page.Controls.Add(grid);
            page.Controls.Add(right);
            return page;
        }

        private static Dictionary<string, TextBox> Fields(string[] names)
        {
            var form = new Dictionary<string, TextBox>();
            foreach (var name in names)
                form[name] = new TextBox { Width = 170 };
            return form;
        }

        private static DataGridView Grid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
            };
        }

        private static Label Header(string text)
        {
            return new Label { Text = text, AutoSize = true, Margin = new Padding(6, 10, 6, 6) };
        }

        private static Button Button(string text)
        {
            return new Button { Text = text, Width = 150, Height = 30, Margin = new Padding(4) };
        }

        private static void AddCrudButtons(TabPage tab, Action load, Action insert, Action update, Action delete)
        {
            var right = tab.Controls["rightPanel"] as Panel;
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 145, FlowDirection = FlowDirection.LeftToRight };
            var refresh = Button("Listele");
            var add = Button("Ekle");
            var edit = Button("Guncelle");
            var remove = Button("Sil");

            refresh.Click += delegate { Safe(load); };
            add.Click += delegate { Safe(delegate { insert(); load(); }); };
            edit.Click += delegate { Safe(delegate { update(); load(); }); };
            remove.Click += delegate
            {
                if (MessageBox.Show("Secili kayit silinsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Safe(delegate { delete(); load(); });
            };

            buttons.Controls.Add(refresh);
            buttons.Controls.Add(add);
            buttons.Controls.Add(edit);
            buttons.Controls.Add(remove);
            right.Controls.Add(buttons);
        }

        private static void Safe(Action action)
        {
            try
            {
                action();
            }
            catch (SqliteException ex)
            {
                MessageBox.Show(ex.Message, "Yerel Veritabani Hatasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Fill(Dictionary<string, TextBox> form, DataGridView grid)
        {
            if (grid.CurrentRow == null)
                return;

            foreach (var item in form)
            {
                if (grid.Columns.Contains(item.Key))
                    item.Value.Text = Convert.ToString(grid.CurrentRow.Cells[item.Key].Value);
            }
        }

        private static object Id(DataGridView grid, string column)
        {
            if (grid.CurrentRow == null)
                throw new InvalidOperationException("Once listeden bir kayit secin.");
            return grid.CurrentRow.Cells[column].Value;
        }

        private static string V(Dictionary<string, TextBox> form, string key)
        {
            var value = form[key].Text.Trim();
            return value.Length == 0 ? null : value;
        }

        private static int IntV(Dictionary<string, TextBox> form, string key)
        {
            return int.Parse(form[key].Text.Trim(), CultureInfo.InvariantCulture);
        }

        private static DateTime DateV(Dictionary<string, TextBox> form, string key)
        {
            return DateTime.Parse(form[key].Text.Trim(), CultureInfo.CurrentCulture).Date;
        }

        private static TimeSpan TimeV(Dictionary<string, TextBox> form, string key)
        {
            return TimeSpan.Parse(form[key].Text.Trim(), CultureInfo.CurrentCulture);
        }

        private static bool BoolV(Dictionary<string, TextBox> form, string key)
        {
            var value = form[key].Text.Trim();
            return value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("evet", StringComparison.OrdinalIgnoreCase);
        }

        private static void EnsureActiveDoctor(int doctorId)
        {
            var active = Db.Scalar("SELECT aktif_mi FROM doktor WHERE doktor_id=@id", Db.P("@id", doctorId));
            if (active == null || active == DBNull.Value)
                throw new InvalidOperationException("Doktor bulunamadi.");
            if (!Convert.ToBoolean(active))
                throw new InvalidOperationException("Aktif olmayan doktora randevu verilemez.");
        }
    }
}
