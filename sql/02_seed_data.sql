USE HastaneBilgiSistemi;
GO

DELETE FROM dbo.recete_kalem;
DELETE FROM dbo.recete;
DELETE FROM dbo.randevu;
DELETE FROM dbo.hasta;
DELETE FROM dbo.doktor;
DELETE FROM dbo.brans;
DBCC CHECKIDENT ('dbo.brans', RESEED, 0);
DBCC CHECKIDENT ('dbo.doktor', RESEED, 0);
DBCC CHECKIDENT ('dbo.hasta', RESEED, 0);
DBCC CHECKIDENT ('dbo.randevu', RESEED, 0);
DBCC CHECKIDENT ('dbo.recete', RESEED, 0);
DBCC CHECKIDENT ('dbo.recete_kalem', RESEED, 0);
GO

DECLARE @i INT = 1;
WHILE @i <= 100
BEGIN
    INSERT INTO dbo.brans(ad, poliklinik_no)
    VALUES (CONCAT('Brans ', FORMAT(@i, '000')), CONCAT('P', FORMAT(@i, '000')));
    SET @i += 1;
END;

SET @i = 1;
WHILE @i <= 100
BEGIN
    INSERT INTO dbo.doktor(ad, soyad, unvan, brans_id, calisma_gunleri, baslangic_saati, bitis_saati, aktif_mi)
    VALUES
    (
        CONCAT('DoktorAd', @i),
        CONCAT('DoktorSoyad', @i),
        CASE WHEN @i % 5 = 0 THEN 'Prof. Dr.' WHEN @i % 3 = 0 THEN 'Doc. Dr.' ELSE 'Uzm. Dr.' END,
        @i,
        CASE WHEN @i % 2 = 0 THEN 'Pazartesi,Carsamba,Cuma' ELSE 'Sali,Persembe' END,
        '09:00',
        '17:00',
        CASE WHEN @i % 20 = 0 THEN 0 ELSE 1 END
    );
    SET @i += 1;
END;

SET @i = 1;
WHILE @i <= 100
BEGIN
    INSERT INTO dbo.hasta(ad, soyad, tc_kimlik, dogum_tarihi, cinsiyet, telefon, adres, kan_grubu)
    VALUES
    (
        CONCAT('HastaAd', @i),
        CONCAT('HastaSoyad', @i),
        RIGHT(CONCAT('10000000000', @i), 11),
        DATEADD(DAY, -(@i * 120), CONVERT(DATE, '2010-01-01')),
        CASE WHEN @i % 2 = 0 THEN 'K' ELSE 'E' END,
        CONCAT('05', RIGHT(CONCAT('000000000', @i * 379), 9)),
        CONCAT('Mahalle ', @i, ', Sokak ', (@i % 25) + 1),
        CASE @i % 8
            WHEN 0 THEN 'A+' WHEN 1 THEN 'A-' WHEN 2 THEN 'B+' WHEN 3 THEN 'B-'
            WHEN 4 THEN 'AB+' WHEN 5 THEN 'AB-' WHEN 6 THEN '0+' ELSE '0-' END
    );
    SET @i += 1;
END;

SET @i = 1;
WHILE @i <= 150
BEGIN
    INSERT INTO dbo.randevu(hasta_id, doktor_id, tarih, saat, durum, sikayet)
    VALUES
    (
        ((@i - 1) % 100) + 1,
        ((@i - 1) % 100) + 1,
        DATEADD(DAY, @i % 45, CONVERT(DATE, '2026-06-01')),
        DATEADD(MINUTE, 30 * (@i % 16), CAST('09:00' AS TIME)),
        CASE WHEN @i % 10 = 0 THEN 'Iptal' WHEN @i % 3 = 0 THEN 'Tamamlandi' ELSE 'Bekliyor' END,
        CONCAT('Ornek sikayet ', @i)
    );
    SET @i += 1;
END;

SET @i = 1;
WHILE @i <= 100
BEGIN
    INSERT INTO dbo.recete(hasta_id, doktor_id, randevu_id, tarih, teshis, notlar)
    SELECT hasta_id, doktor_id, randevu_id, tarih, CONCAT('Teshis ', @i), CONCAT('Doktor notu ', @i)
    FROM dbo.randevu
    WHERE randevu_id = @i;
    SET @i += 1;
END;

SET @i = 1;
WHILE @i <= 200
BEGIN
    INSERT INTO dbo.recete_kalem(recete_id, ilac_adi, doz, kullanim_sekli, sure_gun)
    VALUES
    (
        ((@i - 1) % 100) + 1,
        CONCAT('Ilac ', ((@i - 1) % 30) + 1),
        CASE WHEN @i % 2 = 0 THEN '500mg' ELSE '250mg' END,
        CASE WHEN @i % 3 = 0 THEN 'Gunde 1x1' ELSE 'Gunde 2x1 yemekten sonra' END,
        (@i % 14) + 1
    );
    SET @i += 1;
END;
GO
