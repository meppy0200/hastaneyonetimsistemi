# Hastane Bilgi Sistemi

Visual Studio 2022 ile acilabilen .NET 10 Windows Forms hastane yonetim sistemi.

## Hizli Baslangic

1. Visual Studio 2022'de .NET desktop development workload'u ve .NET 10 SDK kurulu olmalidir.
2. Visual Studio ile `HastaneBilgiSistemi.sln` dosyasini acin.
3. Projeyi calistirin.

Uygulama ilk acilista yerel SQLite veritabani dosyasini otomatik olusturur, tablolari kurar ve ornek verileri ekler. SQL Server veya SSMS kurulumu gerekmez.

## Icerik

- 6 ana tablo: `brans`, `doktor`, `hasta`, `randevu`, `recete`, `recete_kalem`
- PK, FK, UNIQUE ve CHECK constraintler
- Ayni doktor/tarih/saat icin randevu cakismasini engelleyen unique index
- View: `vw_BransAylikRandevuSayisi`, `vw_HastaGecmis`
- Minimum 100 brans, 100 doktor, 100 hasta, 150 randevu, 100 recete, 200 recete kalemi seed verisi
- CRUD ekranlari ve raporlama sekmeleri
- 2026 uyumlulugu icin SDK-style `net10.0-windows` proje yapisi ve `Microsoft.Data.Sqlite`

## Yerel Veritabani

Veriler su dosyada tutulur:

`%LOCALAPPDATA%\HastaneBilgiSistemi\hastane.db`

Veritabani dosyasini silerseniz uygulama sonraki acilista temiz veritabani ve ornek verileri yeniden olusturur.

## Teslim Dosyalari

- SQL scriptleri: `sql/` klasorunde referans olarak durur; uygulama calismak icin bu scriptlere ihtiyac duymaz.
- ER diyagrami: `docs/ER_Diagram.md`
- Teknik dokumantasyon: `docs/Technical_Documentation.md`
- Uygulama kaynak kodlari: `src/`
