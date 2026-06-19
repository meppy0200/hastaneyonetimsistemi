IF DB_ID('HastaneBilgiSistemi') IS NULL
    CREATE DATABASE HastaneBilgiSistemi;
GO

USE HastaneBilgiSistemi;
GO

IF OBJECT_ID('dbo.sp_DoktorGunlukRandevulari', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_DoktorGunlukRandevulari;
IF OBJECT_ID('dbo.vw_BransAylikRandevuSayisi', 'V') IS NOT NULL DROP VIEW dbo.vw_BransAylikRandevuSayisi;
IF OBJECT_ID('dbo.vw_HastaGecmis', 'V') IS NOT NULL DROP VIEW dbo.vw_HastaGecmis;
GO

IF OBJECT_ID('dbo.recete_kalem', 'U') IS NOT NULL DROP TABLE dbo.recete_kalem;
IF OBJECT_ID('dbo.recete', 'U') IS NOT NULL DROP TABLE dbo.recete;
IF OBJECT_ID('dbo.randevu', 'U') IS NOT NULL DROP TABLE dbo.randevu;
IF OBJECT_ID('dbo.hasta', 'U') IS NOT NULL DROP TABLE dbo.hasta;
IF OBJECT_ID('dbo.doktor', 'U') IS NOT NULL DROP TABLE dbo.doktor;
IF OBJECT_ID('dbo.brans', 'U') IS NOT NULL DROP TABLE dbo.brans;
GO

CREATE TABLE dbo.brans
(
    brans_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_brans PRIMARY KEY,
    ad VARCHAR(80) NOT NULL CONSTRAINT UQ_brans_ad UNIQUE,
    poliklinik_no VARCHAR(10) NULL
);

CREATE TABLE dbo.doktor
(
    doktor_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_doktor PRIMARY KEY,
    ad VARCHAR(50) NOT NULL,
    soyad VARCHAR(50) NOT NULL,
    unvan VARCHAR(20) NULL,
    brans_id INT NOT NULL,
    calisma_gunleri VARCHAR(50) NULL,
    baslangic_saati TIME NOT NULL,
    bitis_saati TIME NOT NULL,
    aktif_mi BIT NOT NULL CONSTRAINT DF_doktor_aktif DEFAULT 1,
    CONSTRAINT FK_doktor_brans FOREIGN KEY (brans_id) REFERENCES dbo.brans(brans_id),
    CONSTRAINT CK_doktor_mesai CHECK (baslangic_saati < bitis_saati)
);

CREATE TABLE dbo.hasta
(
    hasta_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_hasta PRIMARY KEY,
    ad VARCHAR(50) NOT NULL,
    soyad VARCHAR(50) NOT NULL,
    tc_kimlik CHAR(11) NOT NULL CONSTRAINT UQ_hasta_tc UNIQUE,
    dogum_tarihi DATE NOT NULL,
    cinsiyet CHAR(1) NOT NULL,
    telefon VARCHAR(15) NULL,
    adres VARCHAR(200) NULL,
    kan_grubu VARCHAR(5) NULL,
    kayit_tarihi DATETIME NOT NULL CONSTRAINT DF_hasta_kayit DEFAULT GETDATE(),
    CONSTRAINT CK_hasta_cinsiyet CHECK (cinsiyet IN ('E', 'K')),
    CONSTRAINT CK_hasta_tc CHECK (tc_kimlik NOT LIKE '%[^0-9]%' AND LEN(tc_kimlik) = 11)
);

CREATE TABLE dbo.randevu
(
    randevu_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_randevu PRIMARY KEY,
    hasta_id INT NOT NULL,
    doktor_id INT NOT NULL,
    tarih DATE NOT NULL,
    saat TIME NOT NULL,
    durum VARCHAR(20) NOT NULL CONSTRAINT DF_randevu_durum DEFAULT 'Bekliyor',
    sikayet VARCHAR(300) NULL,
    olusturma_zamani DATETIME NOT NULL CONSTRAINT DF_randevu_olusturma DEFAULT GETDATE(),
    CONSTRAINT FK_randevu_hasta FOREIGN KEY (hasta_id) REFERENCES dbo.hasta(hasta_id),
    CONSTRAINT FK_randevu_doktor FOREIGN KEY (doktor_id) REFERENCES dbo.doktor(doktor_id),
    CONSTRAINT CK_randevu_durum CHECK (durum IN ('Bekliyor', 'Tamamlandi', 'Iptal'))
);

CREATE UNIQUE INDEX UX_randevu_doktor_tarih_saat
ON dbo.randevu(doktor_id, tarih, saat)
WHERE durum <> 'Iptal';

CREATE TABLE dbo.recete
(
    recete_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_recete PRIMARY KEY,
    hasta_id INT NOT NULL,
    doktor_id INT NOT NULL,
    randevu_id INT NOT NULL,
    tarih DATE NOT NULL CONSTRAINT DF_recete_tarih DEFAULT CONVERT(DATE, GETDATE()),
    teshis VARCHAR(300) NULL,
    notlar VARCHAR(500) NULL,
    CONSTRAINT FK_recete_hasta FOREIGN KEY (hasta_id) REFERENCES dbo.hasta(hasta_id),
    CONSTRAINT FK_recete_doktor FOREIGN KEY (doktor_id) REFERENCES dbo.doktor(doktor_id),
    CONSTRAINT FK_recete_randevu FOREIGN KEY (randevu_id) REFERENCES dbo.randevu(randevu_id),
    CONSTRAINT UQ_recete_randevu UNIQUE (randevu_id)
);

CREATE TABLE dbo.recete_kalem
(
    kalem_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_recete_kalem PRIMARY KEY,
    recete_id INT NOT NULL,
    ilac_adi VARCHAR(100) NOT NULL,
    doz VARCHAR(50) NOT NULL,
    kullanim_sekli VARCHAR(100) NULL,
    sure_gun INT NOT NULL,
    CONSTRAINT FK_recete_kalem_recete FOREIGN KEY (recete_id) REFERENCES dbo.recete(recete_id) ON DELETE CASCADE,
    CONSTRAINT CK_recete_kalem_sure CHECK (sure_gun > 0)
);
GO

CREATE VIEW dbo.vw_BransAylikRandevuSayisi
AS
SELECT
    b.brans_id,
    b.ad AS brans,
    YEAR(r.tarih) AS yil,
    MONTH(r.tarih) AS ay,
    COUNT(*) AS randevu_sayisi,
    SUM(CASE WHEN r.durum = 'Tamamlandi' THEN 1 ELSE 0 END) AS tamamlanan_sayisi,
    SUM(CASE WHEN r.durum = 'Iptal' THEN 1 ELSE 0 END) AS iptal_sayisi
FROM dbo.brans b
INNER JOIN dbo.doktor d ON d.brans_id = b.brans_id
INNER JOIN dbo.randevu r ON r.doktor_id = d.doktor_id
GROUP BY b.brans_id, b.ad, YEAR(r.tarih), MONTH(r.tarih);
GO

CREATE VIEW dbo.vw_HastaGecmis
AS
SELECT
    h.hasta_id,
    h.ad + ' ' + h.soyad AS hasta,
    r.tarih,
    CONVERT(VARCHAR(5), r.saat, 108) AS saat,
    d.unvan + ' ' + d.ad + ' ' + d.soyad AS doktor,
    b.ad AS brans,
    r.durum,
    r.sikayet,
    re.recete_id,
    re.teshis
FROM dbo.hasta h
LEFT JOIN dbo.randevu r ON r.hasta_id = h.hasta_id
LEFT JOIN dbo.doktor d ON d.doktor_id = r.doktor_id
LEFT JOIN dbo.brans b ON b.brans_id = d.brans_id
LEFT JOIN dbo.recete re ON re.randevu_id = r.randevu_id;
GO

CREATE PROCEDURE dbo.sp_DoktorGunlukRandevulari
    @doktor_id INT,
    @tarih DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.randevu_id,
        r.tarih,
        CONVERT(VARCHAR(5), r.saat, 108) AS saat,
        h.hasta_id,
        h.ad + ' ' + h.soyad AS hasta,
        h.telefon,
        r.durum,
        r.sikayet
    FROM dbo.randevu r
    INNER JOIN dbo.hasta h ON h.hasta_id = r.hasta_id
    WHERE r.doktor_id = @doktor_id
      AND r.tarih = @tarih
    ORDER BY r.saat;
END;
GO
