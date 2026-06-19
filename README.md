# Hastane Bilgi Sistemi

Visual Studio 2022 ile açılabilen, .NET 10 Windows Forms tabanlı hastane yönetim sistemi.

## Hızlı Başlangıç

1. Visual Studio 2022 üzerinde **.NET desktop development** workload'u ve .NET 10 SDK kurulu olmalıdır.
2. `HastaneBilgiSistemi.sln` dosyasını Visual Studio ile açın.
3. Projeyi çalıştırın.

Uygulama ilk açılışta yerel SQLite veritabanı dosyasını otomatik oluşturur, tabloları kurar ve örnek verileri ekler.

## İçerik

- 6 ana tablo: `brans`, `doktor`, `hasta`, `randevu`, `recete`, `recete_kalem`
- Primary Key, Foreign Key, UNIQUE ve CHECK kısıtları
- Aynı doktor, tarih ve saat için randevu çakışmasını engelleyen unique index
- View: `vw_BransAylikRandevuSayisi`, `vw_HastaGecmis`
- Minimum 100 branş, 100 doktor, 100 hasta, 150 randevu, 100 reçete ve 200 reçete kalemi seed verisi
- CRUD ekranları ve raporlama sekmeleri
- 2026 uyumluluğu için SDK-style `net10.0-windows` proje yapısı
- Yerel veritabanı için `Microsoft.Data.Sqlite`

## Yerel Veritabanı

Veriler şu dosyada tutulur:

```txt
%LOCALAPPDATA%\HastaneBilgiSistemi\hastane.db
```

Veritabanı dosyasını silerseniz uygulama sonraki açılışta temiz veritabanını ve örnek verileri yeniden oluşturur.

## Teslim Dosyaları

- SQL scriptleri: `sql/` klasöründe referans olarak durur; uygulamanın çalışması için bu scriptlere ihtiyaç yoktur.
- ER diyagramı: `docs/ER_Diagram.md`
- Teknik dokümantasyon: `docs/Technical_Documentation.md`
- Uygulama kaynak kodları: `src/`
