# Hastane Bilgi Sistemi Teknik Dokumantasyon

## Amac

Bu proje hasta kaydi, doktor yonetimi, brans bazli randevu takibi, recete yazimi ve raporlama ihtiyaclarini karsilayan bir hastane bilgi yonetim sistemidir.

## Teknoloji Yigini

- Programlama dili: C#
- Framework: .NET 10 Windows Desktop (`net10.0-windows`)
- Arayuz: Windows Forms
- Veritabani: Yerel SQLite dosyasi
- Veri erisimi: ADO.NET (`Microsoft.Data.Sqlite`)
- IDE: Visual Studio 2022

## Klasor Yapisi

- `HastaneBilgiSistemi.sln`: Visual Studio cozum dosyasi.
- `src/`: Windows Forms uygulama kaynak kodlari.
- `sql/01_schema.sql`: SQL Server icin referans schema scripti.
- `sql/02_seed_data.sql`: SQL Server icin referans seed scripti.
- `sql/03_queries_reports.sql`: JOIN, GROUP BY ve subquery ornekleri.
- `docs/ER_Diagram.md`: Mermaid ER diyagrami.
- `docs/Technical_Documentation.md`: Teknik dokumantasyon.

## Veritabani Tasarimi

Ana tablolar:

- `brans`
- `doktor`
- `hasta`
- `randevu`
- `recete`
- `recete_kalem`

Kisitlar:

- Primary key butun tablolarda vardir.
- Foreign key iliskileri tum bagimli tablolarda tanimlidir.
- CHECK constraintler: `CK_hasta_cinsiyet`, `CK_hasta_tc`, `CK_randevu_durum`, `CK_doktor_mesai`, `CK_recete_kalem_sure`.
- Cakisan randevu onleme: `UX_randevu_doktor_tarih_saat`.

## Kurulum

1. Visual Studio 2022'de .NET desktop development workload'u ve .NET 10 SDK kurulu olmalidir.
2. Visual Studio ile `HastaneBilgiSistemi.sln` dosyasini acin.
3. Projeyi baslatin.

Uygulama ilk acilista `%LOCALAPPDATA%\HastaneBilgiSistemi\hastane.db` dosyasini olusturur. Schema ve seed verileri kod icinden otomatik uygulanir.

## Ekranlar

- Brans CRUD
- Doktor CRUD
- Hasta CRUD
- Randevu CRUD ve musait saat listesi
- Recete CRUD
- Recete kalemi CRUD
- Hasta gecmisi
- Raporlar

## Raporlama

Minimum iki anlamli rapor uygulamada bulunur:

- Doktora gore gunluk/haftalik randevu raporu.
- Brans bazli aylik randevu yogunlugu raporu.

Ek raporlar:

- Hastaya gore recete ve ilac dokumu.
- Hic randevusu olmayan hastalar.

## Is Kurallari

- Randevu durumu `Bekliyor`, `Tamamlandi`, `Iptal` disinda olamaz.
- Ayni doktor icin ayni tarih ve saate ikinci aktif randevu girilemez.
- Aktif olmayan doktora uygulama ekranindan randevu atanamaz.
- Recete bir randevu ile iliskilidir.
- Recete kaleminde kullanim suresi 0'dan buyuk olmalidir.

## 2026 Uyumluluk Notlari

- Proje SDK-style `.csproj` yapisindadir.
- Hedef framework `net10.0-windows` olarak ayarlanmistir.
- SQL Server bagimliligi kaldirilmistir.
- Yerel veritabani icin `Microsoft.Data.Sqlite` NuGet paketi kullanilir.
- Veritabani dosyasi kullanici profilindeki LocalAppData klasorunde saklanir.

## Test Senaryolari

- Ayni TC kimlik numarasina sahip ikinci hasta eklenmeye calisildiginda veritabani hata vermelidir.
- `cinsiyet` alanina `E` veya `K` disinda deger girildiginde CHECK constraint calismalidir.
- Ayni doktor, tarih ve saat icin ikinci randevu eklendiginde unique index hata vermelidir.
- Randevu raporu tarih araligina gore veri getirmelidir.
- Brans yogunluk raporu `vw_BransAylikRandevuSayisi` view'i uzerinden calismalidir.
