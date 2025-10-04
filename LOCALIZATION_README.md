# Blazor WebAssembly Lokalizasyon Eklentisi

Bu proje Blazor WebAssembly uygulamanıza Türkçe ve İngilizce dil desteği eklemek için geliştirilmiştir.

## Özellikler

✅ **Çoklu Dil Desteği**: Türkçe ve İngilizce  
✅ **Browser LocalStorage**: Kullanıcının dil tercihini kaydetme  
✅ **Dinamik Dil Değiştirme**: Sayfayı yenilemeden dil değiştirme  
✅ **Otomatik Yeniden Yükleme**: Dil değişikliğinden sonra sayfa yenilenir  
✅ **Modern UI**: Bootstrap dropdown ile dil seçici  
✅ **Responsive Design**: Mobil uyumlu tasarım  

## Eklenen Dosyalar

### Resource Dosyaları
- `Resources/SharedResource.cs` - Boş kaynak sınıfı
- `Resources/SharedResource.resx` - İngilizce çeviriler (varsayılan)
- `Resources/SharedResource.tr.resx` - Türkçe çeviriler

### Component'ler  
- `Components/CultureSwitcher.razor` - Dil değiştirme komponenti

### Güncellenmiş Dosyalar
- `Program.cs` - Lokalizasyon servisleri eklendi
- `Personal.csproj` - Lokalizasyon paketleri eklendi
- `_Imports.razor` - Gerekli namespace'ler eklendi
- Tüm sayfa dosyaları lokalizasyon desteği ile güncellendi

## Kullanım

### 1. Dil Değiştirici
Navbar'da sağ üst köşedeki globe ikonu ile dil değiştirilebilir:

```razor
<CultureSwitcher />
```

### 2. Çeviri Kullanımı
Sayfalarınızda çevirileri şu şekilde kullanabilirsiniz:

```razor
@inject IStringLocalizer<SharedResource> Localizer

<h1>@Localizer["Home.Title"]</h1>
<p>@Localizer["Home.Description"]</p>
```

### 3. Yeni Çeviri Ekleme

**SharedResource.resx** (İngilizce):
```xml
<data name="MyKey" xml:space="preserve">
  <value>My English Text</value>
</data>
```

**SharedResource.tr.resx** (Türkçe):
```xml
<data name="MyKey" xml:space="preserve">
  <value>Benim Türkçe Metnim</value>
</data>
```

## Desteklenen Diller

| Dil | Kültür Kodu | Bayrak |
|-----|-------------|--------|
| Türkçe | tr-TR | 🇹🇷 |
| İngilizce | en-US | 🇺🇸 |

## Varsayılan Dil

Uygulama varsayılan olarak **Türkçe** başlar. Kullanıcı daha önce bir dil seçmişse, o dil otomatik yüklenir.

## Tarayıcı Desteği

- ✅ Chrome 80+
- ✅ Firefox 75+
- ✅ Safari 13+
- ✅ Edge 80+

## Kurulum

1. Gerekli NuGet paketleri otomatik yüklenir:
   ```
   Microsoft.Extensions.Localization
   ```

2. Projeyi derleyin:
   ```bash
   dotnet build
   ```

3. Çalıştırın:
   ```bash
   dotnet run
   ```

## Kişiselleştirme

### Yeni Dil Ekleme

1. Yeni resource dosyası oluşturun: `SharedResource.[kültür-kodu].resx`
2. `CultureSwitcher.razor`'a yeni dil seçeneği ekleyin
3. `Program.cs`'de varsayılan kültürü güncelleyin

### Çeviri Anahtarları

Mevcut çeviri anahtarları:
- `Nav.*` - Navigasyon menüsü
- `Home.*` - Ana sayfa
- `Contact.*` - İletişim sayfası  
- `Projects.*` - Projeler sayfası
- `About.*` - Hakkında sayfası

## Geliştirici Notları

- Tüm string'ler resource dosyalarında saklanır
- Dil değişikliği localStorage'da saklanır
- Sayfa yenileme gerekir (otomatik yapılır)
- Bootstrap 5.3+ gereklidir dropdown için
- .NET 9.0 ile test edilmiştir

---

**Geliştirici**: Kadir Beşkardeş  
**Lisans**: MIT  
**Versiyon**: 1.0.0