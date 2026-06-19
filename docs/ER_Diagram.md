# Hastane Bilgi Sistemi ER Diyagrami

```mermaid
erDiagram
    brans ||--o{ doktor : "uzmanlik"
    doktor ||--o{ randevu : "verir"
    hasta ||--o{ randevu : "alir"
    hasta ||--o{ recete : "sahiptir"
    doktor ||--o{ recete : "yazar"
    randevu ||--o| recete : "tetikler"
    recete ||--o{ recete_kalem : "icerir"

    brans {
        int brans_id PK
        varchar ad UK
        varchar poliklinik_no
    }

    doktor {
        int doktor_id PK
        varchar ad
        varchar soyad
        varchar unvan
        int brans_id FK
        varchar calisma_gunleri
        time baslangic_saati
        time bitis_saati
        bit aktif_mi
    }

    hasta {
        int hasta_id PK
        varchar ad
        varchar soyad
        char tc_kimlik UK
        date dogum_tarihi
        char cinsiyet
        varchar telefon
        varchar adres
        varchar kan_grubu
        datetime kayit_tarihi
    }

    randevu {
        int randevu_id PK
        int hasta_id FK
        int doktor_id FK
        date tarih
        time saat
        varchar durum
        varchar sikayet
        datetime olusturma_zamani
    }

    recete {
        int recete_id PK
        int hasta_id FK
        int doktor_id FK
        int randevu_id FK
        date tarih
        varchar teshis
        varchar notlar
    }

    recete_kalem {
        int kalem_id PK
        int recete_id FK
        varchar ilac_adi
        varchar doz
        varchar kullanim_sekli
        int sure_gun
    }
```

## Iliskiler

- Bir brans birden fazla doktora baglanabilir.
- Bir hasta birden fazla randevu alabilir.
- Bir doktor birden fazla randevu verebilir.
- Bir randevudan en fazla bir recete olusur.
- Bir recete birden fazla recete kalemi icerir.

## Kritik Kisitlar

- `hasta.cinsiyet` alani yalnizca `E` veya `K` degerlerini kabul eder.
- `randevu.durum` alani `Bekliyor`, `Tamamlandi`, `Iptal` degerlerinden biridir.
- `recete_kalem.sure_gun` pozitif olmak zorundadir.
- `UX_randevu_doktor_tarih_saat` index'i ayni doktor, tarih ve saate ikinci aktif randevuyu engeller.
