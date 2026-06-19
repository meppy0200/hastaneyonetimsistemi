USE HastaneBilgiSistemi;
GO

-- INNER JOIN: Hastanin tum randevulari doktor ve brans bilgisiyle.
SELECT
    h.hasta_id,
    h.ad + ' ' + h.soyad AS hasta,
    r.tarih,
    CONVERT(VARCHAR(5), r.saat, 108) AS saat,
    d.unvan + ' ' + d.ad + ' ' + d.soyad AS doktor,
    b.ad AS brans,
    r.durum
FROM dbo.hasta h
INNER JOIN dbo.randevu r ON r.hasta_id = h.hasta_id
INNER JOIN dbo.doktor d ON d.doktor_id = r.doktor_id
INNER JOIN dbo.brans b ON b.brans_id = d.brans_id
WHERE h.hasta_id = 1
ORDER BY r.tarih, r.saat;

-- LEFT JOIN: Recetesi olmayan randevular da gorunecek sekilde hasta gecmisi.
SELECT
    r.randevu_id,
    h.ad + ' ' + h.soyad AS hasta,
    r.tarih,
    r.durum,
    re.recete_id,
    re.teshis
FROM dbo.randevu r
INNER JOIN dbo.hasta h ON h.hasta_id = r.hasta_id
LEFT JOIN dbo.recete re ON re.randevu_id = r.randevu_id
ORDER BY r.randevu_id;

-- INNER JOIN: Doktora ait bugunku randevu listesi.
DECLARE @bugun DATE = CONVERT(DATE, GETDATE());
SELECT
    d.doktor_id,
    d.ad + ' ' + d.soyad AS doktor,
    r.tarih,
    CONVERT(VARCHAR(5), r.saat, 108) AS saat,
    h.ad + ' ' + h.soyad AS hasta,
    h.telefon,
    r.sikayet
FROM dbo.doktor d
INNER JOIN dbo.randevu r ON r.doktor_id = d.doktor_id
INNER JOIN dbo.hasta h ON h.hasta_id = r.hasta_id
WHERE d.doktor_id = 1
  AND r.tarih = @bugun
ORDER BY r.saat;

-- GROUP BY: Bransa gore aylik randevu sayisi raporu.
SELECT * FROM dbo.vw_BransAylikRandevuSayisi
ORDER BY yil DESC, ay DESC, randevu_sayisi DESC;

-- Cakisma kontrolu: Ayni doktor + tarih + saat icin filtrelenmis unique index vardir.
SELECT name, type_desc
FROM sys.indexes
WHERE object_id = OBJECT_ID('dbo.randevu')
  AND name = 'UX_randevu_doktor_tarih_saat';

-- Subquery: Hic randevusu olmayan hastalar.
SELECT hasta_id, ad, soyad, tc_kimlik
FROM dbo.hasta
WHERE hasta_id NOT IN (SELECT DISTINCT hasta_id FROM dbo.randevu);

-- Stored procedure ornegi.
EXEC dbo.sp_DoktorGunlukRandevulari @doktor_id = 1, @tarih = @bugun;
GO
