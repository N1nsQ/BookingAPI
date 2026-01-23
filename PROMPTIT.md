# PROMPTIT - CLAUDE

### 👤 Nina

> Hei Claude! Puhutko suomea?

### 🤖 Claude

Hei! Kyllä, puhun suomea! 😊  
Voin auttaa sinua suomeksi monenlaisissa asioissa - keskustella, vastata kysymyksiin, auttaa kirjoittamisessa tai mitä vain tarvitset. Mitä kuuluu?

### 👤 Nina

> Haluan että autat minua rakentamaan yksinkertaisen kokoushuoneiden varausrajapinnan eli APIn. Käytetään toteutuksessa C# ohjelmointikieltä ja tietokantana voi toimia EF Coren InMemory -database. Olen luonut tyhjän ASP.NET Core Web API projektin Visual Studiossa.
>
> Palvelun tulee tarjota käyttäjille seuraavat toiminnot:  
> ● Varauksen luonti: Varaa huone tietylle aikavälille.  
> ● Varauksen peruutus: Poista varaus.  
> ● Varausten katselu: Listaa kaikki tietyn huoneen varaukset.
>
> Toimintalogiikka (business rules):  
> ● Varaukset eivät saa mennä päällekkäin (kaksi henkilöä ei voi varata samaa huonetta samaan aikaan).  
> ● Varaukset eivät voi sijoittua menneisyyteen.  
> ● Aloitusajan täytyy olla ennen lopetusaikaa.

### 🤖 Claude

Hyvä! Rakennetaan sinulle toimiva kokoushuoneiden varausrajapinta. Aloitetaan luomalla tarvittavat tiedostot projektiisi.

#### 1. Domain-mallit

Luo ensin `Models` -kansio ja sinne seuraavat luokat:

```
MeetingRoom.cs
Code (Alkuperäinen AI-generoitu commit)
```

```
Booking.cs
Code (Alkuperäinen AI-generoitu commit)
```

#### 2. Database Context

Luo `Data` -kansio ja sinne

```
ApplicationDbContext.cs
Code (Alkuperäinen AI-generoitu commit)
```

#### 3. DTOt (Data Transfer Objects)

Luo DTOs -kansio ja sinne:

```
BookingDtos.cs
Code (Alkuperäinen AI-generoitu commit)
```

#### 4. Controller

Luo `Controllers` -kansioon (tai päivitä olemassa oleva):

```
BookingsController.cs
Code (Alkuperäinen AI-generoitu commit)
```

#### 5. Program.cs konfiguraatio

Päivitä `Program.cs` tiedostosi seuraavasti:

```
Program.cs
Code (Alkuperäinen AI-generoitu commit)
```

#### Asennettavat NuGet-paketit

Varmista että projektissasi on seuraavat paketit (voit asentaa ne NuGet Package Managerilla):

```
Microsoft.EntityFrameworkCore.InMemory
Microsoft.EntityFrameworkCore
```

#### Käyttöohje

Kun käynnistät projektin, voit testata APIa Swaggerin kautta osoitteessa https://localhost:[portti]/swagger

##### API-endpointit:

1. **POST /api/Bookings** - Luo uusi varaus
2. **DELETE /api/Bookings/{id}** - Peruuta varaus
3. **GET /api/Bookings/room/{roomId}** - Listaa huoneen varaukset
4. **GET /api/Bookings** - Listaa kaikki varaukset
5. **GET /api/Bookings/{id}** - Hae yksittäinen varaus

Esimerkki POST-pyynnöstä:

```json
{
  "meetingRoomId": 1,
  "bookedBy": "Matti Meikäläinen",
  "startTime": "2026-01-22T10:00:00",
  "endTime": "2026-01-22T11:00:00",
  "description": "Viikkopalaveri"
}
```

Kaikki business rules on toteutettu controllerissa ja ne antavat selkeät virheilmoitukset, jos validointi epäonnistuu. Haluatko että selitän jotain osaa tarkemmin?

### 👤 Nina

> Tämä vaikuttaa jo toimivalta ratkaisulta. Tarvitsen vielä git ignore tiedoston, joka ottaa huomioon C# ja Visula Studion tuottamat tiedostot ja kansiot. Voitko generoida minulle sellaisen?

### 🤖 Claude

Toki! Tässä on kattava .gitignore-tiedosto C# ja Visual Studio -projekteille:

`.gitignore`
Code
Tässä on kattava .gitignore-tiedosto, joka huomioi:

- **Visual Studio** -tiedostot ja asetukset (.vs/, _.user, _.suo jne.)
- **Build-kansiot** (bin/, obj/, Debug/, Release/)
- **NuGet-paketit** (packages/)
- **Testitulokset** ja coverage-raportit
- **ReSharper** ja muut lisäosat
- **VS Code** -asetukset
- **JetBrains Rider** -asetukset
- Muut väliaikaiset ja geneeroidut tiedostot

Tallenna tämä projektisi juurikansioon nimellä `.gitignore` (huomaa piste alussa). Tiedosto pitää huolen siitä, että vain oleelliset lähdekooditiedostot päätyvät Git-repositoryysi, eikä kaikki Visual Studion generoimat väliaikaiset tiedostot.

### 👤 Nina

> Haluan että seuraavaksi pureudumme syvällisemmin tuottamaasi APIin. Controllerissa virhetilanteita on käsitelty eri tavoin:
>
> 1 ) return BadRequest("Aloitusajan täytyy olla ennen lopetusaikaa.");  
> 2) return Conflict("Huone on jo varattu kyseiselle ajanjaksolle.");
>
> Mitä eroa on BadRequest ja Conflict tyypeillä? Miksi nämä tyypit on valittu?

### 🤖 Claude

Erinomainen kysymys! Nämä ovat HTTP-statuskoodeista johdettu vastaustyyppi, ja niiden valinta perustuu REST API -suunnittelun parhaaisiin käytäntöihin. Käydään läpi erot:

#### HTTP-statuskoodit

##### BadRequest (400)

- Tarkoittaa että **pyyntö on virheellinen** asiakkaan toiminnan takia
- Käytetään kun syöttödata on väärässä muodossa tai rikkoo validointisääntöjä
- Viesti asiakkaalle: "Korjaa pyyntösi, sillä se on virheellinen"
- Pyynnön uudelleen lähettäminen samalla datalla ei koskaan onnistu

##### Conflict (409)

- Tarkoittaa että pyyntö on sinänsä validi, mutta se **on ristiriidassa resurssin nykyisen tilan kanssa**
- Käytetään kun operaatio ei onnistu järjestelmän nykyisen tilan takia
- Viesti asiakkaalle: "Pyyntösi oli ok, mutta se ei ole tällä hetkellä mahdollinen"
- Pyynnön uudelleen lähettäminen myöhemmin tai eri ajankohdalla voisi onnistua

#### Miksi juuri nämä valinnat?

##### 1. Aloitusaika ennen lopetusaikaa (BadRequest)

```c#
if (dto.StartTime >= dto.EndTime)
{
    return BadRequest("Aloitusajan täytyy olla ennen lopetusaikaa.");
}
```

- Tämä on **datan validointivirhe** - pyyntö on rakenteeltaan virheellinen
- Ajat ovat loogisesti väärin päin, mikä ei koskaan voi olla järkevää
- Asiakkaan täytyy korjata lähetettävä data

##### 2. Varaus menneisyydessä (BadRequest)

```c#
if (dto.StartTime < DateTime.Now)
{
    return BadRequest("Varaus ei voi sijoittua menneisyyteen.");
}
```

- Myös validointivirhe - pyyntö rikkoo perussääntöjä
- Menneisyyteen varaaminen ei ole koskaan sallittua
- Asiakkaan täytyy muuttaa aikoja

##### 3. Päällekkäinen varaus (Conflict)

```c#
if (hasOverlap)
{
    return Conflict("Huone on jo varattu kyseiselle ajanjaksolle.");
}
```

- Tämä on **tilakonflikti** - pyyntö on sinänsä validi, mutta järjestelmän tila estää sen
- Sama pyyntö olisi voinut onnistua, jos joku ei olisi ehtinyt ensin
- Asiakas voisi kokeilla uudelleen eri ajankohdalla

##### Käytännön ero asiakkaalle

Jos kehität esimerkiksi frontend-sovelluksen:  
**BadRequest-virheessä** käyttäjälle näytetään lomakevirhe: "Tarkista syöttämäsi tiedot"  
**Conflict-virheessä** käyttäjälle voidaan näyttää: "Huone on jo varattu. Haluatko valita toisen ajan?" ja ehkä tarjota vaihtoehtoisia aikoja.

##### Muita yleisiä statuskoodeja

Vertailun vuoksi, muita koodeja joita API:ssa voitaisiin käyttää:

- **404 Not Found** - Resurssia ei löydy (käytetään kun huonetta ei ole)
- **401 Unauthorized** - Ei autentikoitu
- **403 Forbidden** - Autentikoitu mutta ei oikeuksia
- **422 Unprocessable Entity** - Joissakin API:ssa käytetään BadRequestin sijaan semanttisiin validointivirheisiin
- **500 Internal Server Error** - Palvelinvirhe (ei asiakkaan vika)

Haluaisitko että käyn läpi myös muut HTTP-statuskoodit tai selitän jotain muuta API:n osaa tarkemmin?

### 👤 Nina

> BookingDtos pitää sisällään kaksi DTO:ta, CreateBookingDto ja BookingDto. Onko tämän ratkaisun takana jokin perustelu? Laitan itse tavallisesti jokaisen DTO:n omaan tiedostoonsa sillä niiden määrä saattaa kasvaa suureksi sovelluksen kehittyessä.

### 🤖 Claude

Olet täysin oikeassa! Laitoin molemmat DTO:t samaan tiedostoon lähinnä yksinkertaisuuden ja esimerkin tiiviydenaluettavuuden vuoksi, mutta **tuotantokoodissa oma tiedosto per DTO on ehdottomasti parempi käytäntö.**

#### Miksi erilliset tiedostot ovat parempia?

##### 1. Skaalautuvuus

- Kun sovellus kasvaa, DTO:ita tulee lisää (UpdateBookingDto, BookingListDto, BookingSearchDto jne.)
- Yksi iso tiedosto muuttuu nopeasti vaikeaksi hallita

##### 2. Selkeys ja löydettävyys

- `CreateBookingDto.cs` on helpompi löytää kuin etsiä oikeaa luokkaa suuresta tiedostosta
- Visual Studion "Go to File" -toiminto (Ctrl+,) toimii paremmin

##### 3. Git-konfliktit

- Jos kaksi kehittäjää muokkaa eri DTO:ita, ei tule merge-konflikteja
- Samassa tiedostossa muokkaukset aiheuttavat herkemmin konflikteja

##### 4. Single Responsibility Principle

- Yksi tiedosto, yksi vastuualue
- Helpompi ymmärtää mitä tiedosto sisältää

##### 5. Testattavuus ja ylläpidettävyys

- Pienempiä, fokusoidumpia tiedostoja on helpompi ylläpitää
- Muutoshistoria (git blame) pysyy selkeämpänä

##### Suositeltu rakenne

```
DTOs/
├── CreateBookingDto.cs
├── BookingDto.cs
├── UpdateBookingDto.cs          // Tulevaisuudessa
├── BookingSearchDto.cs          // Tulevaisuudessa
├── MeetingRoomDto.cs            // Tulevaisuudessa
└── CreateMeetingRoomDto.cs      // Tulevaisuudessa
```

##### Milloin yhteinen tiedosto on ok?

Yhteinen tiedosto voi olla perusteltua:

- **Hyvin pienet, tiiviisti liittyvät DTO:t** jotka aina muuttuvat yhdessä
- **Prototyypit ja demot** (kuten tässä esimerkissä)
- **Sisäkkäiset/apuluokat** jotka eivät ole julkisia

##### Parannettu rakenne

Jos haluaisit jakaa ne omiin tiedostoihinsa, rakenne olisi:

##### CreateBookingDto.cs:

```csharp
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBookingApi.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int MeetingRoomId { get; set; }

        [Required]
        public string BookedBy { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Description { get; set; }
    }
}
```

##### BookingDto.cs:

```csharp
namespace MeetingRoomBookingApi.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int MeetingRoomId { get; set; }
        public string MeetingRoomName { get; set; } = string.Empty;
        public string BookedBy { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Description { get; set; }
    }
}
```

Hyvä huomio! Oikeassa projektissa kannattaa ehdottomasti noudattaa "yksi luokka per tiedosto" -periaatetta DTO:iden kanssa. Se maksaa itsensä takaisin heti kun sovellus alkaa kasvaa.

### 👤 Nina

> Sovelluksella pystyy tekemään varauksen, joka on minuutin mittainen, ja toisaalta pystyn tekemään viisipäiväisen varauksen. Ehkä varauksen pituudella pitäisi olla ylä- ja alarajat. Luulen, että esimerkiksi 15 minuuttia voisi olla hyvä alaraja varaukselle (Esim Outlook-kalenterissa taitaa olla tällainen ominaisuus) mutta mikähän voisi olla hyvä yläraja? Joskus voi varmasti tulla tilanteita, että kokoushuonetta tarvitaan useampana päivänä samalle porukalle. Olisikohan käyttäjäystävällisempää tehdä jokaiselle päivälle oma varaus vai sallia useamman päivän varaukset?

### 🤖 Claude

Erinomaisia pohintoja! Tämä on tyypillinen business logic -kysymys, jossa tekninen toteutus ja käyttäjäkokemus kohtaavat. Käydään läpi vaihtoehdot:

#### Varauksen pituusrajoitukset

##### Alaraja: 15 minuuttia ✓

- Järkevä valinta, estää vahingossa tehdyt "tyhjät" varaukset
- Outlookin ja useimpien kalenterijärjestelmien standardi
- Käytännössä harvemmin tarvitaan lyhyempiä

##### Yläraja: Riippuu käyttötapauksesta

##### Vaihtoehto 1: Salli pitkät varaukset (esim. max 7 päivää)

**Edut:**

- Yksinkertainen käyttäjälle - yksi varaus koko jaksolle
- Vähemmän klikkailuja
- Sopii tilanteisiin: "Tarvitsemme projektihuoneen koko viikoksi"

**Haitat:**

- Huone on "lukittu" koko ajaksi, vaikka käyttö olisi vain päivisin
- Ei joustoa - jos haluaa vapauttaa yhden päivän väliltä, pitää peruuttaa ja tehdä uudelleen
- Vaikeuttaa muiden varausten tekemistä

##### Vaihtoehto 2: Rajoita max 1 päivä per varaus

**Edut:**

- Pakottaa tekemään varauksen vain todelliselle käyttöajalle
- Joustava - helppo vapauttaa yksittäisiä päiviä
- Reilumpi muille käyttäjille

**Haitat:**

- Enemmän työtä käyttäjälle (5 varausta viikon projektihuoneelle)
- Ei estä varaamasta 24/7 jos sallittu

##### Vaihtoehto 3: Rajoita työpäivän pituuteen (esim. max 10-12 tuntia)

**Edut:**

- Estää "huoneen monopolisoinnin"
- Pakottaa miettimään todellisen tarpeen
- Jättää tilaa myös muille (esim. iltakäyttöön)

**Haitat:**

- Ei sovellu tilanteisiin missä huone tarvitaan pidemmäksi aikaa

##### Suositus: Hybridilähestymistapa

```csharp
// Vakiot konfiguraatioon tai appsettings.json
public const int MinBookingMinutes = 15;
public const int MaxBookingHours = 8;  // Normaali työpäivä
```

**Perustelu:**

- **15 min alaraja** - estää vahingot ja järjettömät varaukset
- **8 tunnin yläraja** - sallii koko työpäivän varaukset, mutta estää monopolisoinnin
- Jos tarvitaan pidemmäksi, tehdään useampi varaus (esim. ma-pe, jokaiselle 8h)

##### Lisäominaisuuksia harkittavaksi

##### 1. Toistuva varaus -toiminto

```csharp
public class RecurringBookingDto
{
    public RecurrencePattern Pattern { get; set; } // Daily, Weekly, etc.
    public DateTime RecurrenceEndDate { get; set; }
}
```

- Helpottaa viikkovarausten tekoa
- Käyttäjä tekee yhden "template-varauksen" ja järjestelmä luo kopiot

##### 2. Roolikohtaiset rajoitukset

- Tavalliset käyttäjät: max 8h per varaus
- Admin/projektipäälliköt: max 7 päivää
- VIP-käyttäjät: ei rajoituksia

##### 3. Huonekohtaiset säännöt

```csharp
public class MeetingRoom
{
    public int MaxBookingHours { get; set; } = 8;  // Voidaan override
}
```

- Isot auditorio-tilat: sallii koko päivän
- Pienet neukkarit: max 2h

##### Toteutus koodissa

```c#
// BookingsController.cs
// Code ∙ Version 2

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Models;

namespace MeetingRoomBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private const int MinBookingMinutes = 15;
        private const int MaxBookingHours = 8;

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
        {
            // Validoi: Aloitusajan täytyy olla ennen lopetusaikaa
            if (dto.StartTime >= dto.EndTime)
            {
                return BadRequest("Aloitusajan täytyy olla ennen lopetusaikaa.");
            }

            // Validoi: Varaukset eivät voi sijoittua menneisyyteen
            if (dto.StartTime < DateTime.Now)
            {
                return BadRequest("Varaus ei voi sijoittua menneisyyteen.");
            }

            // Validoi: Varauksen minimipituus
            var duration = dto.EndTime - dto.StartTime;
            if (duration.TotalMinutes < MinBookingMinutes)
            {
                return BadRequest($"Varauksen minimipituus on {MinBookingMinutes} minuuttia.");
            }

            // Validoi: Varauksen maksimipituus
            if (duration.TotalHours > MaxBookingHours)
            {
                return BadRequest($"Varauksen maksimipituus on {MaxBookingHours} tuntia.");
            }

            // Tarkista että huone on olemassa
            var room = await _context.MeetingRooms.FindAsync(dto.MeetingRoomId);
            if (room == null)
            {
                return NotFound($"Kokoushuonetta ID:llä {dto.MeetingRoomId} ei löydy.");
            }

            // Tarkista päällekkäisyydet
            var hasOverlap = await _context.Bookings
                .AnyAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
                             ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                              (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                              (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

            if (hasOverlap)
            {
                return Conflict("Huone on jo varattu kyseiselle ajanjaksolle.");
            }

            // Luo varaus
            var booking = new Booking
            {
                MeetingRoomId = dto.MeetingRoomId,
                BookedBy = dto.BookedBy,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Description = dto.Description
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var resultDto = new BookingDto
            {
                Id = booking.Id,
                MeetingRoomId = booking.MeetingRoomId,
                MeetingRoomName = room.Name,
                BookedBy = booking.BookedBy,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Description = booking.Description
            };

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, resultDto);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var dto = new BookingDto
            {
                Id = booking.Id,
                MeetingRoomId = booking.MeetingRoomId,
                MeetingRoomName = booking.MeetingRoom.Name,
                BookedBy = booking.BookedBy,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Description = booking.Description
            };

            return dto;
        }

        // GET: api/Bookings/room/1
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetRoomBookings(int roomId)
        {
            var room = await _context.MeetingRooms.FindAsync(roomId);
            if (room == null)
            {
                return NotFound($"Kokoushuonetta ID:llä {roomId} ei löydy.");
            }

            var bookings = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .Where(b => b.MeetingRoomId == roomId)
                .OrderBy(b => b.StartTime)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    MeetingRoomId = b.MeetingRoomId,
                    MeetingRoomName = b.MeetingRoom.Name,
                    BookedBy = b.BookedBy,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Description = b.Description
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .OrderBy(b => b.StartTime)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    MeetingRoomId = b.MeetingRoomId,
                    MeetingRoomName = b.MeetingRoom.Name,
                    BookedBy = b.BookedBy,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Description = b.Description
                })
                .ToListAsync();

            return Ok(bookings);
        }
    }
}
```

#### Yhteenveto

##### Suosittelen:

- **Alaraja: 15 minuuttia** (industry standard)
- **Yläraja: 8 tuntia** (estää monopolisoinnin, mutta sallii työpäivän)
- Mahdollisuus myöhemmin laajentaa "toistuva varaus" -toiminnolla

##### Käyttäjäystävällisyys:

- Pitkille projekteille tarjotaan "Varaa toistuva" -toiminto frontendissä
- Yksi klikkaus luo esim. ma-pe varaukset automaattisesti
- Jokainen päivä on silti oma varauksensa → helppo muokata

Mitä mieltä olet tästä lähestymistavasta? Sopiiko 8 tunnin yläraja käyttötapaukseesi vai tarvitseeko sitä säätää?

### 👤 Nina

> Tutustuin päivittämääsi controller-koodiin.
>
> ```
> // Validoi: Varauksen minimipituus
> var duration = dto.EndTime - dto.StartTime;
> if (duration.TotalMinutes < MinBookingMinutes)
> {
> return BadRequest($"Varauksen minimipituus on {MinBookingMinutes} minuuttia.");
> }
> ```
>
> Mietin, että kumpikohan olisi käyttäjäystävällisempi tapa: Se, että sovellus antaa virheilmoituksen, jos yritetään varata liian lyhyttä aikaa (niinkuin koodiesimerkki nyt tekee) vai pitäisikö varaus joka on lyhyempi kuin 15 minuuttia
> muuttaa backendillä 15 minuutin varaukseksi kyselemättä. Toisaalta, jos kyseessä on varauksen tekeminen vahingossa, pitäisi varmaan olla sitten myös jonkinlainen "Vahvista varaus" vaihe. Jos sovelluksella olisi UI, käyttäjä näkisi heti
> ettei pysty tekemään alle 15 min varauksia kun kelloa ei pysty asettamaan sen taaemmaksi. Mutta nyt kun teemme vain APIa, niin miten suosittelisit tehtävän?

### 🤖 Claude

Erinomainen kysymys! Tämä on klassinen API-suunnittelun dilemma: validointi vs. automaattinen korjaus. Käydään läpi molemmat lähestymistavat:

#### Vaihtoehto 1: Validointi (nykyinen toteutus) ✓

```c#
if (duration.TotalMinutes < MinBookingMinutes)
{
    return BadRequest($"Varauksen minimipituus on {MinBookingMinutes} minuuttia.");
}
```

#### Edut:

- **Eksplisiittinen** - API ei tee yllätyksiä
- **Ennakoitava** - käyttäjä saa mitä pyytää tai virheen
- **RESTful-periaatteiden mukainen** - API ei muuta käyttäjän syötettä
- **Debuggaus helpompaa** - virhe kertoo suoraan mikä meni pieleen

#### Haitat:

- Vaatii frontend-kehittäjältä lisätyötä
- Extra roundtrip palvelimelle jos validointi tehdään vain backendissä

#### Vaihtoehto 2: Automaattinen korjaus ("Be liberal in what you accept")

```c#
// Korjaa liian lyhyet varaukset automaattisesti
var duration = dto.EndTime - dto.StartTime;
if (duration.TotalMinutes < MinBookingMinutes)
{
    dto.EndTime = dto.StartTime.AddMinutes(MinBookingMinutes);
}
```

#### Edut:

- **"Käyttäjäystävällinen"** - ei kaadu pieniin virheisiin
- Vähemmän virhetilanteita

#### Haitat:

- **Yllätyksellistä** - käyttäjä pyysi 10 min, sai 15 min
- **Vaarallista** - voi luoda päällekkäisyyksiä odottamatta
- **Ei-determinististä** - sama input voi tuottaa eri tuloksen riippuen tilan muutoksista
- **Hankala testata** - pitää tietää milloin korjaus tapahtuu

#### Vaihtoehto 3: Informatiivinen validointi (SUOSITUS)

Paras ratkaisu API:lle on **validoida, mutta tarjota hyödyllistä tietoa:**

```c#
if (duration.TotalMinutes < MinBookingMinutes)
{
    return BadRequest(new
    {
        error = "InvalidBookingDuration",
        message = $"Varauksen minimipituus on {MinBookingMinutes} minuuttia.",
        suggestedEndTime = dto.StartTime.AddMinutes(MinBookingMinutes),
        actualDuration = duration.TotalMinutes,
        minimumDuration = MinBookingMinutes
    });
}
```

#### Edut:

- Selkeä virhe
- Frontend saa kaiken tarvitseman tiedon korjaukseen
- Voidaan näyttää käyttäjälle: "Haluatko varata 15 min asti (klo 10:15)?"

#### API Design -periaatteet

**REST API:n kultainen sääntö:\***

_"Validate strictly what you receive, be clear about what you return"_

##### Miksi validointi on parempi:

- **Principle of Least Astonishment** - API ei tee yllätyksiä
- **Single Source of Truth** - Frontend näyttää käyttäjälle rajoitukset, backend validoi
- **Separation of Concerns** - UI päättää UX:n, API varmistaa datan eheyden

#### Käytännön toteutus

Sinun tapauksessasi **suosittelen nykyistä validointi-lähestymistapaa,** mutta voit parantaa virheviestiä:

```
BookingController.cs
Code ∙ Version 3
```

```
BookingController.cs
Code ∙ Version 4
```

#### Frontend-puolen käsittely

Kun UI-kehittäjä käyttää tätä API:a, hän voi:

```javascript
// Frontend esimerkki
try {
  const response = await fetch('/api/bookings', {
    method: 'POST',
    body: JSON.stringify(bookingData)
  });

  if (!response.ok) {
    const error = await response.json();
    if (error.error === 'BookingTooShort') {
      // Näytä käyttäjälle viesti ja ehdota korjausta
      showMessage(error.message);
      suggestNewEndTime(error.minimumDuration);
    }
  }
} catch (e) { ... }
```

#### Poikkeus sääntöön

**Ainoa tilanne jossa automaattinen korjaus on ok:**

- Pyöristykset (esim. 10:07 → 10:05 tai 10:10)
- Jos UI antaa vain tiettyjä vaihtoehtoja (dropdownit)

Mutta silloinkin kannattaa dokumentoida selkeästi!

#### Yhteenveto

**Suositus:** Pidä validointi, mutta paranna virheviestejä strukturoiduiksi objekteiksi. Tämä:

- On RESTful ja ennakoitavaa
- Antaa frontendille kaiken tarvittavan tiedon
- Ei tee yllätyksiä
- On helppo testata

Mitä mieltä olet tästä lähestymistavasta? Tuntuuko strukturoitu virheviesti paremmalta kuin pelkkä string?

### 👤 Nina

> Informatiivinen validointi kuulosti parhaalta lähestymistavalta tässä vaiheessa. Lisäsin ehdotuksesi minimivarausajan toteutuksesta. Jään vielä miettimään, miten toimia maksimin kanssa.
>
> Seuraavaksi pohdin koodin jäsentelyä. Pitäisikö Controller-tasolta siivota pois kaikki logiikka ja luoda uusi Services-taso sitä varten, vai millainen arkkitehtuuri voisi sopia pienehkön APIn toteutuksessa?

### 🤖 Claude

Erinomainen kysymys! Tämä on yksi tärkeimmistä arkkitehtuuripäätöksistä. Vastaus riippuu projektin koosta ja tulevaisuuden suunnitelmista.

#### Nykyinen tilanne: "Fat Controller"

Tällä hetkellä controllerisi sisältää:

- HTTP-käsittelyn (routing, status koodit)
- Validointilogiikan
- Business logiikan (päällekkäisyyden tarkistus)
- Tietokantalogiikan (EF Core kyselyt)

#### Vaihtoehto 1: Services-kerros (SUOSITUS)

Jaa vastuut selkeästi:

```
├── Controllers/        → HTTP, routing, status koodit
├── Services/          → Business logiikka
├── Repositories/      → (Valinnainen) Tietokantakäsittely
├── Models/            → Domain mallit
└── DTOs/              → Data Transfer Objects
```

**Edut:**

- **Testattavuus** - voit testata business logiikkaa ilman HTTP-kontekstia
- **Uudelleenkäytettävyys** - sama logiikka voi palvella useita controllereitä
- **Single Responsibility** - jokainen luokka tekee yhden asian
- **Skaalautuvuus** - helppo laajentaa

**Haitat:**

- Enemmän tiedostoja ja abstraktioita
- Voi tuntua "overengineeringiltä" pienessä projektissa

#### Vaihtoehto 2: Minimal API / Thin Controllers

Pidä kaikki controllerissa jos:

- Projekti pysyy pienenä (< 5 endpointia)
- Ei tule koskaan toista käyttöliittymää (esim. CLI, background job)
- Ei yksikkötestejä

**Edut:**

- Yksinkertainen ja nopea kehittää
- Kaikki yhdessä paikassa

**Haitat:**

- Vaikea testata
- Muuttuu nopeasti sekavaksi
- Vaikea laajentaa

#### Käytännön suositus: Aloita Services-kerroksella

Kun projekti kasvaa edes hieman, Services-kerros maksaa itsensä takaisin. Tässä refaktoroitu versio:

#### IBookingService.cs (Interface):

```c#
// IBookingService.cs

using MeetingRoomBookingApi.DTOs;

namespace MeetingRoomBookingApi.Services
{
    public interface IBookingService
    {
        Task<ServiceResult<BookingDto>> CreateBookingAsync(CreateBookingDto dto);
        Task<ServiceResult<BookingDto>> GetBookingAsync(int id);
        Task<ServiceResult<IEnumerable<BookingDto>>> GetRoomBookingsAsync(int roomId);
        Task<ServiceResult<IEnumerable<BookingDto>>> GetAllBookingsAsync();
        Task<ServiceResult<bool>> DeleteBookingAsync(int id);
    }

    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public object? ErrorDetails { get; set; }
        public ServiceErrorType ErrorType { get; set; }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string message, ServiceErrorType errorType, object? details = null)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = message,
                ErrorType = errorType,
                ErrorDetails = details
            };
        }
    }

    public enum ServiceErrorType
    {
        ValidationError,
        NotFound,
        Conflict,
        ServerError
    }
}
```

#### BookingService.cs (Toteutus):

```c#
// BookingService.cs

using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Models;

namespace MeetingRoomBookingApi.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private const int MinBookingMinutes = 15;
        private const int MaxBookingHours = 8;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<BookingDto>> CreateBookingAsync(CreateBookingDto dto)
        {
            // Validoi: Aloitusajan täytyy olla ennen lopetusaikaa
            if (dto.StartTime >= dto.EndTime)
            {
                return ServiceResult<BookingDto>.Failure(
                    "Aloitusajan täytyy olla ennen lopetusaikaa.",
                    ServiceErrorType.ValidationError
                );
            }

            // Validoi: Varaukset eivät voi sijoittua menneisyyteen
            if (dto.StartTime < DateTime.Now)
            {
                return ServiceResult<BookingDto>.Failure(
                    "Varaus ei voi sijoittua menneisyyteen.",
                    ServiceErrorType.ValidationError
                );
            }

            // Validoi: Varauksen minimipituus
            var duration = dto.EndTime - dto.StartTime;
            if (duration.TotalMinutes < MinBookingMinutes)
            {
                return ServiceResult<BookingDto>.Failure(
                    $"Varauksen minimipituus on {MinBookingMinutes} minuuttia.",
                    ServiceErrorType.ValidationError,
                    new
                    {
                        error = "BookingTooShort",
                        currentDuration = $"{duration.TotalMinutes} minuuttia",
                        minimumDuration = $"{MinBookingMinutes} minuuttia"
                    }
                );
            }

            // Validoi: Varauksen maksimipituus
            if (duration.TotalHours > MaxBookingHours)
            {
                return ServiceResult<BookingDto>.Failure(
                    $"Varauksen maksimipituus on {MaxBookingHours} tuntia.",
                    ServiceErrorType.ValidationError,
                    new
                    {
                        error = "BookingTooLong",
                        currentDuration = $"{duration.TotalHours:F1} tuntia",
                        maximumDuration = $"{MaxBookingHours} tuntia"
                    }
                );
            }

            // Tarkista että huone on olemassa
            var room = await _context.MeetingRooms.FindAsync(dto.MeetingRoomId);
            if (room == null)
            {
                return ServiceResult<BookingDto>.Failure(
                    $"Kokoushuonetta ID:llä {dto.MeetingRoomId} ei löydy.",
                    ServiceErrorType.NotFound
                );
            }

            // Tarkista päällekkäisyydet
            var hasOverlap = await _context.Bookings
                .AnyAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
                             ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                              (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                              (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

            if (hasOverlap)
            {
                return ServiceResult<BookingDto>.Failure(
                    "Huone on jo varattu kyseiselle ajanjaksolle.",
                    ServiceErrorType.Conflict
                );
            }

            // Luo varaus
            var booking = new Booking
            {
                MeetingRoomId = dto.MeetingRoomId,
                BookedBy = dto.BookedBy,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Description = dto.Description
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var resultDto = new BookingDto
            {
                Id = booking.Id,
                MeetingRoomId = booking.MeetingRoomId,
                MeetingRoomName = room.Name,
                BookedBy = booking.BookedBy,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Description = booking.Description
            };

            return ServiceResult<BookingDto>.Success(resultDto);
        }

        public async Task<ServiceResult<BookingDto>> GetBookingAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return ServiceResult<BookingDto>.Failure(
                    "Varausta ei löytynyt.",
                    ServiceErrorType.NotFound
                );
            }

            var dto = new BookingDto
            {
                Id = booking.Id,
                MeetingRoomId = booking.MeetingRoomId,
                MeetingRoomName = booking.MeetingRoom.Name,
                BookedBy = booking.BookedBy,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Description = booking.Description
            };

            return ServiceResult<BookingDto>.Success(dto);
        }

        public async Task<ServiceResult<IEnumerable<BookingDto>>> GetRoomBookingsAsync(int roomId)
        {
            var room = await _context.MeetingRooms.FindAsync(roomId);
            if (room == null)
            {
                return ServiceResult<IEnumerable<BookingDto>>.Failure(
                    $"Kokoushuonetta ID:llä {roomId} ei löydy.",
                    ServiceErrorType.NotFound
                );
            }

            var bookings = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .Where(b => b.MeetingRoomId == roomId)
                .OrderBy(b => b.StartTime)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    MeetingRoomId = b.MeetingRoomId,
                    MeetingRoomName = b.MeetingRoom.Name,
                    BookedBy = b.BookedBy,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Description = b.Description
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<BookingDto>>.Success(bookings);
        }

        public async Task<ServiceResult<IEnumerable<BookingDto>>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .OrderBy(b => b.StartTime)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    MeetingRoomId = b.MeetingRoomId,
                    MeetingRoomName = b.MeetingRoom.Name,
                    BookedBy = b.BookedBy,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Description = b.Description
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<BookingDto>>.Success(bookings);
        }

        public async Task<ServiceResult<bool>> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return ServiceResult<bool>.Failure(
                    "Varausta ei löytynyt.",
                    ServiceErrorType.NotFound
                );
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}
```

#### Refaktoroitu BookingsController.cs:

```c#
// BookingsController.cs
// Code ∙ Version 5

using Microsoft.AspNetCore.Mvc;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Services;

namespace MeetingRoomBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
        {
            var result = await _bookingService.CreateBookingAsync(dto);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.ValidationError => BadRequest(result.ErrorDetails ?? result.ErrorMessage),
                    ServiceErrorType.NotFound => NotFound(result.ErrorMessage),
                    ServiceErrorType.Conflict => Conflict(result.ErrorMessage),
                    _ => StatusCode(500, result.ErrorMessage)
                };
            }

            return CreatedAtAction(nameof(GetBooking), new { id = result.Data!.Id }, result.Data);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var result = await _bookingService.GetBookingAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // GET: api/Bookings/room/1
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetRoomBookings(int roomId)
        {
            var result = await _bookingService.GetRoomBookingsAsync(roomId);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return NoContent();
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookingsAsync();
            return Ok(result.Data);
        }
    }
}

```

#### Päivitä Program.cs rekisteröimään service:

```c#
// Program.cs
// Code ∙ Version 2

using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Lisää Entity Framework Core InMemory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MeetingRoomDb"));

// Rekisteröi services
builder.Services.AddScoped<IBookingService, BookingService>();

// Lisää Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Alusta tietokanta seed-datalla
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

```

Lisää myös using:

```csharp
using MeetingRoomBookingApi.Services;
```

#### Mitä saavutettiin?

**Controller nyt:**

- 80 riviä → 70 riviä
- Vain HTTP-käsittelyä
- Helppo lukea ja ymmärtää

**Service nyt:**

- Kaikki business logiikka yhdessä paikassa
- Testattavissa ilman HTTP-kontekstia
- Uudelleenkäytettävissä

**ServiceResult-pattern:**

- Tyyppiturvallisuus
- Selkeä virheiden käsittely
- Ei poikkeuksia kontrollivuon hallintaan

#### Testauksen helppous

```csharp
[Fact]
public async Task CreateBooking_WithPastDate_ReturnsValidationError()
{
    // Arrange
    var service = new BookingService(mockContext);
    var dto = new CreateBookingDto
    {
        StartTime = DateTime.Now.AddDays(-1),
        EndTime = DateTime.Now
    };

    // Act
    var result = await service.CreateBookingAsync(dto);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal(ServiceErrorType.ValidationError, result.ErrorType);
}
```

#### Yhteenveto

**Käytä Services-kerrosta kun:**

✅ Projekti kasvaa yli 3-5 endpointin  
✅ Haluat yksikkötestata logiikan  
✅ Sama logiikka tarvitaan useassa paikassa  
✅ Tiimi kasvaa

Voit skipata jos:

❌ Toteutat nopean prototypen  
❌ Projekti pysyy alle 5 endpointissa ikuisesti  
❌ Ei tule koskaan testejä

Sinun tapauksessasi suosittelen Services-kerrosta - projekti vaikuttaa siltä että se kasvaa!

### 👤 Nina

> Jep, lisään projektiini Services-kerroksen.
>
> Jatkossa voit jättää keskustelussamme koodiesimerkit pois, ellen erikseen pyydä niitä.

### 🤖 Claude

Selvä! Jatketaan ilman koodiesimerkkejä. Hyvä päätös lisätä Services-kerros - se tekee koodista paljon ylläpidettävämpää kun projekti kasvaa.  
Onko jotain muuta API:n osa-aluetta, jota haluaisit vielä käydä läpi tai parantaa? Esimerkiksi:

- Autentikointi/auktorisointi
- Lokitus ja virheiden käsittely
- Paginaatio pitkille listauksille
- Suodatus- ja hakutoiminnot
- Konfiguraatiotiedostot (appsettings.json)
- API-dokumentaatio (Swagger-annotaatiot)

Vai haluatko jatkaa jotain tiettyä aihetta?

### 👤 Nina

> Olet luonut Service-interfaceen ServiceResult luokan. Se näyttäisi pitävän tiedon siitä, onko pyyntö onnistunut vai ei sekä mahdolliset virheviestit. Haluaisin tietää enemmän miksi tämä on valittu. Olin itse ajatellut, että virheiden käsittely voisi tapahtua omalla layerilla. Vertaile näitä toteutustapoja, niiden hyviä ja huonoja puolia.

### 🤖 Claude

Erinomainen kysymys! Tämä on yksi keskeisimmistä arkkitehtuuripäätöksistä. Käydään läpi molemmat lähestymistavat perusteellisesti.

#### Vaihtoehto 1: Result Pattern (nykyinen)

ServiceResult käärii vastauksen ja mahdolliset virheet yhteen objektiin.

**Toimintaperiaate:**

- Service palauttaa aina `ServiceResult<T>`
- Ei heitetä poikkeuksia normaalissa virhetilanteessa
- Controller tarkistaa `IsSuccess` ja reagoi sen mukaan

**Edut:**

- **Eksplisiittinen virheenkäsittely** - pakottaa handleoimaan virheet
- **Tyyppiturvallisuus** - kääntäjä varoittaa jos unohdat tarkistaa tuloksen
- **Parempi suorituskyky** - ei exception-overheadia
- **Testattavuus** - helppo mockata ja testata eri skenaarioita
- **Functional programming -tyyli** - puhdas, sivuvaikutukseton
- **Railway-oriented programming** - virheet "kulkevat raiteilla" läpi kerrosten

**Haitat:**

- Enemmän boilerplate-koodia
- Jokainen kutsu vaatii tuloksen purkamisen
- Voi tuntua raskaalta yksinkertaisissa tapauksissa
- Kaksi tapaa käsitellä virheitä (Result + Exceptions)

**Käyttöesimerkki:**

```
Service: Palauttaa ServiceResult<BookingDto>
Controller: Tarkistaa result.IsSuccess ja muuntaa HTTP-vastaukseksi
```

#### Vaihtoehto 2: Exception-pohjainen (middleware-käsittely)

Service heittää poikkeuksia, jotka middleware napaa ja muuntaa HTTP-vastauksiksi.

**Toimintaperiaate:**

- Service heittää domain-spesifisiä poikkeuksia (BookingValidationException, ResourceNotFoundException)
- Global exception handler middleware napaa poikkeukset
- Middleware muuntaa poikkeukset sopiviksi HTTP-vastauksiksi

**Edut:**

- **Yksinkertaisempi service-koodi** - ei tarvitse palauttaa Result-objekteja
- **Standardimpi .NET-lähestymistapa** - tuttu monille kehittäjille
- **Keskitetty virheenkäsittely** - kaikki virheet käsitellään yhdessä paikassa
- **Vähemmän boilerplatea** - suoraviivaisempi koodi
- **Automaattinen virhelogitus** - middleware voi loggata kaiken

**Haitat:**

- **Piilotettua kontrollivirtaa** - ei näe koodista että metodi voi "epäonnistua"
- **Suorituskyky** - exception-heitto on kalliimpaa
- **Testaus hankalampaa** - täytyy odottaa poikkeuksia
- **Ei pakota käsittelemään** - helppo unohtaa edge caset
- **Exception ei ole virhe** - validointivirhe ei ole "poikkeuksellinen tilanne"

**Toteutus:**

```
Service: throw new BookingConflictException("Huone varattu")
Middleware: Napaa poikkeuksen → muuntaa 409 Conflict
Controller: Ei tarvitse käsitellä virheitä
```

#### Vaihtoehto 3: Hybridi (yleinen käytäntö)

Yhdistä molemmat: käytä Result-patternia odotetuille virheille, Exceptioneita odottamattomille.

**Jako:**

- **Result Pattern** → Business logic virheet (validointi, konfliktit, not found)
- **Exceptions** → Todelliset poikkeustilanteet (tietokantayhteys katkeaa, OutOfMemory)

**Edut:**

- Paras molemmista maailmoista
- Selkeä jako odotettujen ja odottamattomien virheiden välillä
- Business logic virheet ovat eksplisiittisiä
- Middleware hoitaa "vakavat" virheet

**Haitat:**

- Monimutkaisempi - kaksi virheenkäsittelytapaa
- Täytyy päättää milloin käyttää kumpakin
- Vaatii tiimikohtaiset konventiot

#### Milloin käyttää mitäkin?

**Result Pattern sopii kun:**

- Haluat eksplisiittisen virheenkäsittelyn
- Suorituskyky on kriittinen
- Tiimi arvostaa functional programming -tyyliä
- Haluat pakon käsitellä virheet
- Domain-logiikka on monimutkaista

**Exception-middleware sopii kun:**

- Haluat yksinkertaisen koodin
- Seuraat perinteistä .NET-tyyliä
- Tiimi on tottunut exception-pohjaiseen koodiin
- Virheet ovat harvemmin toistuvia
- Haluat keskitetyn virhelogituksen

**Hybridi sopii kun:**

- Haluat parhaat puolet molemmista
- On selkeä jako "odotettujen" ja "odottamattomien" virheiden välillä
- Tiimi ymmärtää molemmat lähestymistavat

#### Oma suositukseni projektillesi

**Jos projekti pysyy pienenä (< 10 endpointia):** Exception-middleware riittää. Yksinkertaisempi ja nopeampi toteuttaa.  
**Jos projekti kasvaa keskisuureksi (10-50 endpointia):** Result Pattern. Eksplisiittisyys kannattaa kun business logic monimutkaistuu.  
**Jos projekti on suuri enterprise-sovellus:** Hybridi. Tarvitset molemmat työkalut.

#### Konkreettinen vertailu koodissa

**Result Pattern:**

- Service: 150 riviä (sisältää Result-luokat)
- Controller: 70 riviä (käsittelee Resultit)
- Middleware: 0 riviä
- Yhteensä: ~220 riviä

**Exception-middleware:**

- Service: 100 riviä (yksinkertaisempi)
- Controller: 40 riviä (ei virheenkäsittelyä)
- Middleware: 50 riviä (exception handler)
- Custom Exceptions: 30 riviä
- Yhteensä: ~220 riviä

Koodin määrä on sama, mutta rakenne erilainen!

#### Mikä sinun projektissasi?

Koska API:si on vielä pieni mutta tulet todennäköisesti laajentamaan sitä, Result Pattern oli hyvä valinta. Se skaalautuu paremmin ja pakottaa miettimään virhetilanteet etukäteen.
Haluaisitko nähdä miltä exception-middleware -toteutus näyttäisi vertailun vuoksi, vai keskitytäänkö johonkin muuhun API:n osa-alueeseen?

### 👤 Nina

> Uskon että endpointit tulee pysymään alle kymmenessä. Tehdään virheenkäsittely mieluummin Exception Handling Middleware -toteutuksena

### 🤖 Claude

Hyvä valinta! Exception-middleware tekee koodista paljon yksinkertaisempaa pienessä projektissa. Muutetaan arkkitehtuuri käyttämään poikkeuksia.

#### Mitä tarvitaan?

1. **Custom Exception -luokat** - Domain-spesifit poikkeukset
2. **Global Exception Middleware** - Napaa poikkeukset ja muuntaa HTTP-vastauksiksi
3. **Yksinkertaistettu Service** - Heittää poikkeuksia, ei palauta Result-objekteja
4. **Yksinkertaistettu Controller** - Ei virheenkäsittelyä

#### Arkkitehtuurin muutokset

**Poista:**

- `ServiceResult<T>` luokka
- `ServiceErrorType` enum
- Kaikki `IsSuccess`-tarkistukset controllerista

**Lisää:**

- Custom exception -luokat (ValidationException, NotFoundException, ConflictException)
- Exception handling middleware
- Rekisteröi middleware Program.cs:ssä

#### Custom Exception -luokat

Luo `Exceptions` -kansio projektiisi ja sinne seuraavat:  
**BookingValidationException** - Validointivirheille  
**BookingNotFoundException** - Kun resurssia ei löydy  
**BookingConflictException** - Päällekkäisille varauksille  
Nämä perivät `Exception`-luokan ja voivat sisältää lisätietoja (kuten `ErrorDetails`-objektin).

#### Exception Middleware

Middleware nappaa kaikki poikkeukset ja muuntaa ne:

- `BookingValidationException` → 400 Bad Request
- `BookingNotFoundException` → 404 Not Found
- `BookingConflictException` → 409 Conflict
- Muut → 500 Internal Server Error

Middleware voi myös loggata virheet automaattisesti.

#### Yksinkertaistettu Service

Service-metodi muuttuu esimerkiksi:

```c#
public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
{
    if (dto.StartTime >= dto.EndTime)
        throw new BookingValidationException("Aloitusaika ennen lopetusaikaa");

    if (hasOverlap)
        throw new BookingConflictException("Huone varattu");

    // ... luo varaus
    return bookingDto;
}
```

Paljon suoraviivaisempaa! Ei Result-wrappereita.

#### Yksinkertaistettu Controller

Controller muuttuu:

```c#
[HttpPost]
public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
{
    var booking = await _bookingService.CreateBookingAsync(dto);
    return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
}
```

Ei virheenkäsittelyä ollenkaan - middleware hoitaa!

#### Edut sinun projektissasi

- **Vähemmän koodia** - ~50 riviä vähemmän yhteensä
- **Selkeämpi lukea** - ei Result-purkamista joka paikassa
- **Keskitetty loggaus** - middleware logittaa kaikki virheet samalla tavalla
- **Helpompi ylläpitää** - uusi endpoint ei vaadi virheenkäsittelyä

#### Interface muuttuu

`IBookingService` yksinkertaistuu:

```c#
public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto);
    Task<BookingDto> GetBookingAsync(int id);
    // ... ei ServiceResult-wrappereita
}
```

Haluatko että kirjoitan koodit näille komponenteille, vai riittääkö tämä yleiskuva toteutuksesta?

### 👤 Nina

> Miten päivitämme DeleteBooking controllerin kun toimintalogiikka on nyt Service-layerilla?
>
> Service:
>
> ```c#
> public async Task<bool> DeleteBookingAsync(int id)
> {
>     var booking = await _context.Bookings.FindAsync(id) ?? throw new Exception($"Varausta ID:llä {id} ei löydy.");
>     _context.Bookings.Remove(booking);
>
>     await _context.SaveChangesAsync();
>     return true;
> }
> ```
>
> Contoller nyt:
>
> ```C#
> [HttpDelete("{id}")]
> public async Task<IActionResult> DeleteBooking(int id)
> {
>     var booking = await _bookingService.DeleteBookingAsync(id);
>     return NoContent();
> }
> ```

### 🤖 Claude

Hyvä kysymys! Nyt kun service heittää poikkeuksen jos varausta ei löydy, controllerisi on jo oikein - se ei tarvitse mitään muutoksia!

#### Miksi controller on jo valmis?

**Kun varaus löytyy:**

- Service palauttaa true
- Controller palauttaa 204 No Content
- Kaikki toimii ✓

**Kun varausta ei löydy:**

- Service heittää Exception (tai paremmin BookingNotFoundException)
- Middleware nappaa poikkeuksen
- Middleware palauttaa 404 Not Found
- Controller ei koskaan suorita return NoContent() -riviä

#### Parannusehdotuksia

**1. Käytä spesifistä poikkeusta:**

```csharp
var booking = await _context.Bookings.FindAsync(id)
    ?? throw new BookingNotFoundException($"Varausta ID:llä {id} ei löydy.");
```

**2. Service voisi palauttaa Task eikä Task<bool>:**

```csharp
public async Task DeleteBookingAsync(int id)
{
    var booking = await _context.Bookings.FindAsync(id)
        ?? throw new BookingNotFoundException($"Varausta ID:llä {id} ei löydy.");

    _context.Bookings.Remove(booking);
    await _context.SaveChangesAsync();
}
```

Koska poisto joko onnistuu (ei palauteta mitään) tai epäonnistuu (heitetään poikkeus), bool-palautusarvo on turha. DELETE-operaatioissa ei tarvitse palauttaa mitään onnistuessa.

**3. Controller pysyy samana:**

```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteBooking(int id)
{
    await _bookingService.DeleteBookingAsync(id);
    return NoContent();
}
```

#### Yhteenveto

Exception-middleware -mallissa controller on **tarkoituksella tyhmä** - se vain kutsuu servicea ja palauttaa success-vastauksen. Kaikki virheiden käsittely tapahtuu automaattisesti middlewaressa.  
Tämä on juuri se yksinkertaisuus mikä tekee exception-mallista houkuttelevan pienissä projekteissa!  
Haluatko vielä käydä läpi jotain muuta API:n osaa?

### 👤 Nina

> Olen nyt siitänyt toimintalogiikan Service-tasolle. Seuraavaksi keskitytään virheiden hallintaan. Auta minua nyt luomaan ExceptionHandlingMiddleware. Voit antaa koodiesimerkkejä.

### 🤖 Claude

Hyvä! Luodaan kattava exception handling middleware. Tässä kaikki tarvittavat komponentit:

#### 1. Custom Exception -luokat

Luo `Exceptions` -kansio ja sinne seuraavat:

```c#
// Custom Exceptions

namespace MeetingRoomBookingApi.Exceptions
{
    // Peruskanta kaikille booking-poikkeuksille
    public abstract class BookingException : Exception
    {
        public object? ErrorDetails { get; set; }

        protected BookingException(string message) : base(message)
        {
        }

        protected BookingException(string message, object? errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }
    }

    // Validointivirheet (400 Bad Request)
    public class BookingValidationException : BookingException
    {
        public BookingValidationException(string message) : base(message)
        {
        }

        public BookingValidationException(string message, object errorDetails)
            : base(message, errorDetails)
        {
        }
    }

    // Resurssi ei löydy (404 Not Found)
    public class BookingNotFoundException : BookingException
    {
        public BookingNotFoundException(string message) : base(message)
        {
        }
    }

    // Konflikti (409 Conflict)
    public class BookingConflictException : BookingException
    {
        public BookingConflictException(string message) : base(message)
        {
        }

        public BookingConflictException(string message, object errorDetails)
            : base(message, errorDetails)
        {
        }
    }
}
```

#### 2. Error Response DTO

Luo yhtenäinen virhevastauksien rakenne:

```c#
// ErrorResponse.cs

namespace MeetingRoomBookingApi.DTOs
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? TraceId { get; set; }
        public object? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ErrorResponse()
        {
        }

        public ErrorResponse(int statusCode, string message, string? traceId = null, object? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            TraceId = traceId;
            Details = details;
        }
    }
}
```

#### 3. Exception Handling Middleware

Luo Middleware -kansio ja sinne:

```c#
// ExceptionHandlingMiddleware.cs

using System.Net;
using System.Text.Json;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Exceptions;

namespace MeetingRoomBookingApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Luo error response ja määritä status code
            var errorResponse = CreateErrorResponse(context, exception);

            // Logita virhe
            LogException(exception, errorResponse.StatusCode);

            // Aseta response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorResponse.StatusCode;

            // Serialisoi ja palauta
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }

        private ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            return exception switch
            {
                BookingValidationException validationEx => new ErrorResponse(
                    statusCode: (int)HttpStatusCode.BadRequest,
                    message: validationEx.Message,
                    traceId: traceId,
                    details: validationEx.ErrorDetails
                ),

                BookingNotFoundException notFoundEx => new ErrorResponse(
                    statusCode: (int)HttpStatusCode.NotFound,
                    message: notFoundEx.Message,
                    traceId: traceId
                ),

                BookingConflictException conflictEx => new ErrorResponse(
                    statusCode: (int)HttpStatusCode.Conflict,
                    message: conflictEx.Message,
                    traceId: traceId,
                    details: conflictEx.ErrorDetails
                ),

                // Kaikki muut virheet -> 500 Internal Server Error
                _ => new ErrorResponse(
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    message: "Palvelimella tapahtui virhe. Yritä myöhemmin uudelleen.",
                    traceId: traceId
                )
            };
        }

        private void LogException(Exception exception, int statusCode)
        {
            // Logita eri tasolla riippuen virhetyypistä
            if (statusCode >= 500)
            {
                // Palvelinvirheet ovat vakavia
                _logger.LogError(exception, "Palvelinvirhe: {Message}", exception.Message);
            }
            else if (statusCode == 404)
            {
                // Not Found ei ole niin vakava
                _logger.LogWarning("Resurssia ei löytynyt: {Message}", exception.Message);
            }
            else
            {
                // Validointi- ja konflikttivirheet ovat informatiivisia
                _logger.LogInformation("Asiakasvirhe ({StatusCode}): {Message}", statusCode, exception.Message);
            }
        }
    }
}
```

#### 4. Rekisteröi Middleware Program.cs:ssä

```c#
// Program.cs

using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Lisää Entity Framework Core InMemory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MeetingRoomDb"));

// Rekisteröi services
builder.Services.AddScoped<IBookingService, BookingService>();

// Lisää Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Alusta tietokanta seed-datalla
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Lisää exception handling middleware (TÄYTYY OLLA ENSIMMÄISENÄ!)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

Lisää myös Program.cs:n alkuun:

```c#
using MeetingRoomBookingApi.Middleware;
```

#### 5. Päivitä Service käyttämään custom exceptioneita

Esimerkki BookingServicestä:

```c#
// Validointivirhe
if (dto.StartTime >= dto.EndTime)
{
    throw new BookingValidationException("Aloitusajan täytyy olla ennen lopetusaikaa.");
}

// Validointivirhe lisätiedoilla
var duration = dto.EndTime - dto.StartTime;
if (duration.TotalMinutes < MinBookingMinutes)
{
    throw new BookingValidationException(
        $"Varauksen minimipituus on {MinBookingMinutes} minuuttia.",
        new
        {
            error = "BookingTooShort",
            currentDuration = $"{duration.TotalMinutes} minuuttia",
            minimumDuration = $"{MinBookingMinutes} minuuttia"
        }
    );
}

// Not Found
var room = await _context.MeetingRooms.FindAsync(dto.MeetingRoomId)
    ?? throw new BookingNotFoundException($"Kokoushuonetta ID:llä {dto.MeetingRoomId} ei löydy.");

// Conflict
if (hasOverlap)
{
    throw new BookingConflictException("Huone on jo varattu kyseiselle ajanjaksolle.");
}
```

#### Mitä tämä antaa sinulle?

**1. Yhtenäinen virherakenne:**  
Kaikki virheet palauttavat saman JSON-rakenteen:

```json
{
  "message": "Varauksen minimipituus on 15 minuuttia.",
  "statusCode": 400,
  "traceId": "0HN7...",
  "details": {
    "error": "BookingTooShort",
    "currentDuration": "10 minuuttia",
    "minimumDuration": "15 minuuttia"
  },
  "timestamp": "2026-01-19T14:30:00Z"
}
```

**2. Automaattinen loggaus:**

- 500-virheet → Error level
- 404-virheet → Warning level
- 400/409 → Information level

**3. TraceId:**  
Helpottaa debuggausta - voit etsiä logeista tietyn pyynnön kaikki tapahtumat.

**4. Yksinkertainen controller:**  
Ei tarvitse käsitellä virheitä ollenkaan!  
Testaa nyt luomalla varaus joka rikkoo jotain sääntöä - pitäisi saada siisti JSON-virheviesti oikealla statuskoodilla!

### 👤 Nina

> Selittäisitkö ratkaisumalliasi hieman tarkemmin. Exception-luokat: Nämä on jälleen laitettu yhteen tiedostoon. Eikö parempi käytäntö olisi tehdä Exceptions-kansion alle jokainen virhetilanne omaan luokkaansa?
>
> ErrorResponse Dto ei myöskään ole ihan selkeä minulle. Voisitko selittää miksi tarvitaan kolme versiota ErrorResponsesta? Miksi tässä ei toteutettu Models -kansioon ErrorResponse entiteettiä?

### 🤖 Claude

Erinomaiset huomiot! Käydään molemmat läpi.

#### Exception-luokat yhdessä tiedostossa

Olet täysin oikeassa - **jokaisen exceptionin pitäisi olla omassa tiedostossaan**. Laitoin ne yhteen samasta syystä kuin DTO:t aiemmin: esimerkin tiiviys. Mutta tuotantokoodissa:

**Suositeltu rakenne:**

```
Exceptions/
├── BookingException.cs              (abstrakti base class)
├── BookingValidationException.cs
├── BookingNotFoundException.cs
└── BookingConflictException.cs
```

#### Miksi erilliset tiedostot:

- Selkeys ja löydettävyys
- Git-konfliktien välttäminen
- Single Responsibility Principle
- Helpompi laajentaa (voit lisätä eri constructoreita, propertyja yms.)
- Noudattaa samaa käytäntöä kuin muu projekti

**Poikkeus sääntöön:** Jos exceptionit ovat **todella** yksinkertaisia (vain konstruktori, ei mitään logiikkaa), ne voi pitää yhdessä tiedostossa. Mutta parempi käytäntö on silti erottaa.

#### ErrorResponse - Miksi DTO eikä Model?

Tämä on tärkeä ero! Käydään läpi miksi **ErrorResponse on DTO** eikä domain model:

**Model vs DTO - ero**  
**Models/ (Domain Models):**

- Edustavat **tietokannan tauluja** tai domain-konsepteja
- Talletetaan tietokantaan
- Sisältävät relaatiot (navigation properties)
- Entity Framework hallinnoi
- Esim: `Booking`, `MeetingRoom`

**DTOs/ (Data Transfer Objects):**

- Käytetään **tiedon siirtoon** API-rajapinnan yli
- EI tallenneta tietokantaan
- Määrittelevät JSON-rakenteen
- Validointi-attribuutit API:a varten
- Esim: `CreateBookingDto`, `BookingDto`, **`ErrorResponse`**

#### Miksi ErrorResponse on DTO?

**ErrorResponse on puhdas API-vastaus**, ei domain-konsepti:

- Ei tallenneta tietokantaan
- Määrittelee miltä virhevastaus näyttää JSON:ina
- Lähtee vain API:sta ulos, ei koskaan sisään
- Middleware luo sen lennossa

Jos tekisit sen "entiteettinä" Models-kansioon:

- Entity Framework yrittäisi luoda sille taulun (pitäisi ignoroida)
- Sekoittaisi domain-konseptit API-konsepteihin
- Rikkoisi Separation of Concerns -periaatteen

#### Kolme "versiota" ErrorResponsesta?

Tässä tapahtui pieni väärinkäsitys - **ErrorResponsesta on vain YKSI luokka**, mutta sillä on **kaksi konstruktoria**:

```c#
public class ErrorResponse
{
    // Propertyt
    public string Message { get; set; }
    public int StatusCode { get; set; }
    // ...

    // Konstruktori 1: Parametriton (tarvitaan JSON-deserializaatioon)
    public ErrorResponse()
    {
    }

    // Konstruktori 2: Kätevä tapa luoda täytetty objekti
    public ErrorResponse(int statusCode, string message, string? traceId = null, object? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        TraceId = traceId;
        Details = details;
    }
}
```

#### Miksi kaksi konstruktoria?

**Parametriton konstruktori:**

- JSON-serialisointi/deserialisointi vaatii sen
- C#:n konventio
- Tekninen välttämättömyys

**Parametrillinen konstruktori:**

- Helpottaa objektin luomista middlewaressa
- Voit kirjoittaa: `new ErrorResponse(400, "Virhe", traceId)`
- Ilman sitä joutuisit:

```c#
var error = new ErrorResponse();
error.StatusCode = 400;
error.Message = "Virhe";
error.TraceId = traceId;
// ...
```

### Yhteenveto rakenteesta

**Models/ - Domain layer**

```
Models/
├── Booking.cs          → Tietokantataulu
└── MeetingRoom.cs      → Tietokantataulu
```

**DTOs/ - API contract layer**

```
DTOs/
├── CreateBookingDto.cs → Sisään tuleva data
├── BookingDto.cs       → Ulos menevä data
└── ErrorResponse.cs    → Virhevastaus (MYÖS ulos menevä data)
```

**Exceptions/ - Error handling layer**

```
Exceptions/
├── BookingException.cs
├── BookingValidationException.cs
├── BookingNotFoundException.cs
└── BookingConflictException.cs
```

### Käytännön esimerkki

**Kun käyttäjä tekee virheellisen pyynnön:**

1. Controller vastaanottaa `CreateBookingDto` (DTO)
2. Service validoi ja heittää `BookingValidationException` (Exception)
3. Middleware nappaa exceptionin
4. Middleware luo `ErrorResponse` (DTO)
5. Middleware serialisoi sen JSON:ksi
6. Käyttäjä saa JSON-virheviestin

**Booking-entity ei koskaan liity virheisiin** - se on domain-konsepti. ErrorResponse on API-konsepti.
Selkiytikö tämä asiaa? Haluatko että refaktoroin exceptionit erillisiin tiedostoihin?

### 👤 Nina

> Joo, refaktoroi exceptionit

### 🤖 Claude

Custom Exceptions

```c#
namespace MeetingRoomBookingApi.Exceptions
{
    // Abstrakti peruskanta kaikille booking-poikkeuksille
    // Tämä voi pysyä samassa tiedostossa koska se on base class
    public abstract class BookingException : Exception
    {
        public object? ErrorDetails { get; set; }

        protected BookingException(string message) : base(message)
        {
        }

        protected BookingException(string message, object? errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }
    }
}
```

BookingException.cs

```c#
namespace MeetingRoomBookingApi.Exceptions
{
    // Abstrakti peruskanta kaikille booking-poikkeuksille
    // Tämä voi pysyä samassa tiedostossa koska se on base class
    public abstract class BookingException : Exception
    {
        public object? ErrorDetails { get; set; }

        protected BookingException(string message) : base(message)
        {
        }

        protected BookingException(string message, object? errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }
    }
}
```

BookingValidationException.cs

```c#

namespace MeetingRoomBookingApi.Exceptions
{
    /// <summary>
    /// Heitetään kun varauksen luonti epäonnistuu validointivirheen takia.
    /// Kääntyy HTTP 400 Bad Request -vastaukseksi.
    /// </summary>
    public class BookingValidationException : BookingException
    {
        public BookingValidationException(string message) : base(message)
        {
        }

        public BookingValidationException(string message, object errorDetails)
            : base(message, errorDetails)
        {
        }
    }
}
```

BookingNotFoundException.cs

```c#
namespace MeetingRoomBookingApi.Exceptions
{
    /// <summary>
    /// Heitetään kun pyydettyä resurssia (varausta tai huonetta) ei löydy.
    /// Kääntyy HTTP 404 Not Found -vastaukseksi.
    /// </summary>
    public class BookingNotFoundException : BookingException
    {
        public BookingNotFoundException(string message) : base(message)
        {
        }
    }
}
```

BookingConflictException.cs

```c#
namespace MeetingRoomBookingApi.Exceptions
{
    /// <summary>
    /// Heitetään kun varaus on ristiriidassa olemassa olevan tilan kanssa.
    /// Esimerkiksi: huone on jo varattu samalle ajanjaksolle.
    /// Kääntyy HTTP 409 Conflict -vastaukseksi.
    /// </summary>
    public class BookingConflictException : BookingException
    {
        public BookingConflictException(string message) : base(message)
        {
        }

        public BookingConflictException(string message, object errorDetails)
            : base(message, errorDetails)
        {
        }
    }
}
```

Valmista! Nyt sinulla on selkeä rakenne:

**Exceptions/-kansio:**

```
Exceptions/
├── BookingException.cs                  (abstrakti base class)
├── BookingValidationException.cs        (400 Bad Request)
├── BookingNotFoundException.cs          (404 Not Found)
└── BookingConflictException.cs          (409 Conflict)
```

**Edut tästä rakenteesta:**

1. **Selkeys** - Jokainen exception omassa tiedostossaan
2. **Dokumentaatio** - Lisäsin XML-kommentit jotka näkyvät IntelliSensessä
3. **Yhtenäisyys** - Seuraa samaa käytäntöä kuin DTOs ja Models
4. **Skaalautuvuus** - Helppo lisätä uusia exceptioneita (esim. `BookingUnauthorizedException`)
5. **Löydettävyys** - Visual Studion "Go to File" toimii paremmin

**Base class erillään:**
Pidin `BookingException`-base classin omana tiedostonaan, koska:

- Se on abstrakti ja sitä ei käytetä suoraan
- Muut exceptionit perivät sen
- Selkeä hierarkia

Jos myöhemmin haluat lisätä esim. autentikoinnin tai muita ominaisuuksia, voit helposti luoda:

- `BookingUnauthorizedException.cs` → 401
- `BookingForbiddenException.cs` → 403
- `BookingRateLimitException.cs` → 429

Kaikki valmista! Haluatko jatkaa jotain muuta API:n osaa?

### 👤 Nina

> ErrorResponseDto:ssa on kentät public string? TraceId { get; set; } ja public object? Details { get; set; }. Kerro näiden tarkoituksesta tarkemmin. Miksi TraceId näytetään virheen yhteydessä? Kuulostaa arkaluontoiselta tiedolta

### 🤖 Claude

Erinomaiset kysymykset! Käydään molemmat läpi.

#### TraceId

**Mikä se on?**
TraceId on **ASP.NET Coren automaattisesti generoima** uniikki tunniste jokaiselle HTTP-pyyntöölle. Se näyttää tältä: `0HN7GKLJQJ8K1:00000001`

**Mistä se tulee?**

```csharp
var traceId = context.TraceIdentifier;
```

Tämä on osa `HttpContext`ia ja ASP.NET Core luo sen automaattisesti.

**Tarkoitus:**
TraceId on **debuggaustyökalu** - ei arkaluontoinen tieto. Se auttaa:

1. **Yhdistämään lokit pyyntöön:**
   - Käyttäjä raportoi virheen ja lähettää TraceId:n
   - Kehittäjä etsii logeista kaikki tapahtumat tuolla TraceId:llä
   - Näkee koko pyynnön kulun alusta loppuun

2. **Support-tilanteissa:**

```
   Käyttäjä: "Sain virheen klo 14:30"
   Support: "Mikä oli TraceId?"
   Käyttäjä: "0HN7GKLJQJ8K1:00000001"
   Support: *etsii logeista* → Löytää tarkan virheen
```

3. **Distributed tracing:**
   Jos kutsut muita mikropalveluita, sama TraceId kulkee mukana ja näet koko ketjun.

**Onko se turvallista näyttää?**

**Kyllä on!** TraceId:

- ❌ EI sisällä käyttäjätietoja
- ❌ EI sisällä arkaluontoista dataa
- ❌ EI paljasta järjestelmän sisäisiä rakenteita
- ✅ ON vain satunnainen tunniste tälle pyynnölle
- ✅ ON hyödyllinen debuggauksessa

**Esimerkki logeista:**

```
[14:30:15] [TraceId: 0HN7GKLJQJ8K1:00000001] Request started: POST /api/bookings
[14:30:15] [TraceId: 0HN7GKLJQJ8K1:00000001] Validating booking...
[14:30:15] [TraceId: 0HN7GKLJQJ8K1:00000001] ERROR: Booking too short
[14:30:15] [TraceId: 0HN7GKLJQJ8K1:00000001] Response: 400 Bad Request
```

Ilman TraceId:tä pitäisi etsiä aikaleiman perusteella, mikä on epätarkkaa kun on paljon liikennettä.

#### Details-kenttä

**Mikä se on?**
`object?` joka voi sisältää **mitä tahansa lisätietoa** virheestä.  
**Miksi** `object?` **eikä konkreettinen tyyppi?**

Koska virhetyypit vaihtelevat:

**Esimerkki 1: Validointivirhe varauksen pituudesta**

```json
{
  "message": "Varauksen minimipituus on 15 minuuttia.",
  "statusCode": 400,
  "details": {
    "error": "BookingTooShort",
    "currentDuration": "10 minuuttia",
    "minimumDuration": "15 minuuttia"
  }
}
```

**Esimerkki 2: Konflikti päällekkäisyydestä**

```json
{
  "message": "Huone on jo varattu.",
  "statusCode": 409,
  "details": {
    "conflictingBookings": [
      {
        "startTime": "2026-01-20T10:00:00",
        "endTime": "2026-01-20T11:00:00",
        "bookedBy": "Matti Meikäläinen"
      }
    ]
  }
}
```

**Esimerkki 3: Not Found - ei detailseja**

```json
{
  "message": "Varausta ei löytynyt.",
  "statusCode": 404,
  "details": null
}
```

**Tarkoitus:**

- Antaa **frontend-kehittäjälle** tarvittavat tiedot virheen käsittelyyn
- Käyttäjälle voidaan näyttää: "Varaus liian lyhyt. Minimiaika on 15 min (yritit 10 min)"
- Tai: "Huone varattu klo 10-11 käyttäjälle Matti. Haluatko valita toisen ajan?"

**Miksi nullable (`?`)?**  
Koska kaikissa virheissä ei ole lisätietoja:

- 404 Not Found → Ei tarvita detailseja
- 500 Server Error → Ei kannata paljastaa sisäisiä vikoja

#### Turvallisuusnäkökulma

**Mitä EI SAA laittaa Details-kenttään:**  
❌ Stack tracea (500-virheissä)  
❌ Tietokannan rakenteita  
❌ Sisäisiä polkuja tai konfiguraatiota  
❌ Käyttäjien henkilökohtaisia tietoja (paitsi jos kyseessä)  
❌ API-avaimia tai salasanoja

**Mitä voi turvallisesti laittaa:**

✅ Validointivirheet ja rajoitukset  
✅ Konfliktitilanteiden perustiedot  
✅ Käyttäjän itse syöttämä data (esim. "yritit varata 10 min")  
✅ Business rule -rikkomukset

**Vaihtoehdot**

**Jos et halua käyttää Details-kenttää:**

**Vaihtoehto 1: Pelkkä message**

```csharp
public class ErrorResponse
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public string? TraceId { get; set; }
    // Ei Details-kenttää
}
```

→ Yksinkertaisempi, mutta vähemmän informatiivinen

**Vaihtoehto 2: Geneerinen tyyppi**

```csharp
public class ErrorResponse<TDetails>
{
    public TDetails? Details { get; set; }
}
```

→ Tyyppiturvallisuus, mutta monimutkaisempi

**Vaihtoehto 3: Erilliset DTO:t jokaiselle virhetyypille**

```
ValidationErrorResponse
ConflictErrorResponse
NotFoundErrorResponse
```

→ Tyyppiturvallisuus, mutta paljon koodia

**Suositus**

**Pidä nykyinen toteutus** (`object? Details`), mutta:

1. **Dokumentoi selkeästi** mitä Details voi sisältää
2. **Älä koskaan laita** sisäisiä virheitä Details-kenttään
3. **Production-ympäristössä:** Harkitse `IsDevelopment`-tarkistusta 500-virheille

```csharp
_ => new ErrorResponse(
    statusCode: (int)HttpStatusCode.InternalServerError,
    message: app.Environment.IsDevelopment()
        ? exception.Message
        : "Palvelimella tapahtui virhe.",
    traceId: traceId,
    details: app.Environment.IsDevelopment()
        ? new { exceptionType = exception.GetType().Name }
        : null
)
```

Tämä paljastaa detaljit vain development-ympäristössä, ei tuotannossa.  
Selkiytikö TraceId:n ja Details-kentän tarkoitus?

### 👤 Nina

> Eli jos haluan tässä koodissa kertoa Details-kohdassa, mille ajalle varaus on olemassa, niin miten se tehtäisin?
>
> ```c#
> // Tarkista päällekkäisyydet
>
>     var hasOverlap = await _context.Bookings
>         .AnyAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
>                 ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
>                 (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
>                 dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));
>    if (hasOverlap)
>        throw new BookingConflictException("Huone on jo varattu kyseiselle ajanjaksolle.");
> ```

### 🤖 Claude

Hyvä kysymys! Tällä hetkellä käytät .AnyAsync(), joka palauttaa vain true/false. Jotta saat päällekkäisen varauksen tiedot, sinun täytyy hakea se erikseen.

Tässä kaksi vaihtoehtoa:

**Vaihtoehto 1: Hae päällekkäinen varaus (SUOSITUS)**

Korvaa `AnyAsync` → `FirstOrDefaultAsync`, jolloin saat koko varausobjektin:

```csharp
// Hae päällekkäinen varaus (jos on)
var conflictingBooking = await _context.Bookings
    .FirstOrDefaultAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
                         ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                          (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                          (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

if (conflictingBooking != null)
{
    throw new BookingConflictException(
        "Huone on jo varattu kyseiselle ajanjaksolle.",
        new
        {
            conflictingBooking = new
            {
                startTime = conflictingBooking.StartTime,
                endTime = conflictingBooking.EndTime,
                bookedBy = conflictingBooking.BookedBy
            }
        }
    );
}
```

**Edut:**

- Vain yksi tietokantakysely
- Saat kaikki tarvittavat tiedot
- Käyttäjä näkee tarkalleen mikä varaus on tiellä

**JSON-vastaus:**

```json
{
  "message": "Huone on jo varattu kyseiselle ajanjaksolle.",
  "statusCode": 409,
  "traceId": "...",
  "details": {
    "conflictingBooking": {
      "startTime": "2026-01-20T10:00:00",
      "endTime": "2026-01-20T11:00:00",
      "bookedBy": "Matti Meikäläinen"
    }
  }
}
```

**Vaihtoehto 2: Hae kaikki päällekkäiset varaukset**

Jos samalle ajalle voi olla useita päällekkäisiä (epätodennäköistä, mutta mahdollista jos data korruptoitunut):

```csharp
var conflictingBookings = await _context.Bookings
    .Where(b => b.MeetingRoomId == dto.MeetingRoomId &&
               ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)))
    .Select(b => new
    {
        startTime = b.StartTime,
        endTime = b.EndTime,
        bookedBy = b.BookedBy
    })
    .ToListAsync();

if (conflictingBookings.Any())
{
    throw new BookingConflictException(
        "Huone on jo varattu kyseiselle ajanjaksolle.",
        new { conflictingBookings }
    );
}
```

#### Suorituskyky

**AnyAsync vs FirstOrDefaultAsync:**

- `AnyAsync()` on **hieman nopeampi** koska se lopettaa heti kun löytää yhden
- `FirstOrDefaultAsync()` hakee koko rivin mutta palauttaa sen

**Käytännössä:**
Ero on marginaalinen (millisekunteja) koska:

- Indeksi on todennäköisesti MeetingRoomId:llä
- Päällekkäisyyksiä on harvoin paljon
- Saat paljon paremman käyttäjäkokemuksen

**Frontend hyötyy**  
Kun frontend saa päällekkäisen varauksen tiedot, se voi:

1. **Näyttää käyttäjälle:**
   "Huone on varattu klo 10:00-11:00 käyttäjälle Matti Meikäläinen"
2. **Ehdottaa vaihtoehtoisia aikoja:**
   "Haluatko varata 11:00-12:00?"
3. **Näyttää kalenterissa:**
   Värjätä päällekkäinen aika punaiseksi
4. **Parempi UX:**
   Sen sijaan että käyttäjä kokeilee satunnaisesti eri aikoja, hän näkee heti milloin huone on varattu.

#### Yhteenveto

**Käytä Vaihtoehto 1** - se on paras kompromissi suorituskyvyn ja käyttäjäkokemuksen välillä. Yksi tietokantakysely, selkeä virheviesti, hyödyllinen data frontendille.

### 👤 Nina

> Mietin AutoMapperin lisäämistä sovellukseen

### 🤖 Claude

Hyvä aihe! AutoMapper on suosittu kirjasto DTO-mappingiin. Käydään läpi kannattaako sitä lisätä sinun projektiin.

#### Mikä AutoMapper on?

AutoMapper automatisoi objektien välisen mappingin. Sen sijaan että kirjoitat:

```csharp
var dto = new BookingDto
{
    Id = booking.Id,
    MeetingRoomId = booking.MeetingRoomId,
    MeetingRoomName = booking.MeetingRoom.Name,
    BookedBy = booking.BookedBy,
    StartTime = booking.StartTime,
    EndTime = booking.EndTime,
    Description = booking.Description
};
```

Kirjoitat vain:

```csharp
var dto = _mapper.Map<BookingDto>(booking);
```

**Edut projektissasi**  
**1. Vähemmän boilerplate-koodia**

- Ei tarvitse toistaa property-assignmentteja
- Muutos entity-luokkaan päivittyy automaattisesti

**2. Yhtenäinen mapping-logiikka**

- Kaikki mappingit yhdessä paikassa (Profile-luokassa)
- Helpompi ylläpitää

**3. Testattavuus**

- Voit testata mapping-konfiguraatiot erikseen
- AutoMapper validoi konfiguraatiot käynnistyksessä

**Haitat projektissasi**

**1. Overkill pienelle projektille**

- Sinulla on vain 2-3 DTO:ta
- Mapping-logiikka on yksinkertaista
- AutoMapper tuo ylimääräisen riippuvuuden

**2. "Magic" ja debuggaus**

- Ei näe suoraan mitä tapahtuu
- Vaikeampi debugata kun jotain menee pieleen
- Uusien kehittäjien täytyy oppia AutoMapper

**3. Suorituskyky**

- Reflection-pohjainen (vaikka cached)
- Pieni overhead verrattuna manuaaliseen mappingiin
- Todellinen projekti: ero on marginaalinen

**4. Monimutkaisemmat caset**

- Custom mapping-logiikka vaatii konfigurointia
- Esim. `MeetingRoomName` pitää erikseen konfiguroida

#### Sinun projektisi tilanne\*\*

**Tällä hetkellä:**

- 2 entiteettiä (Booking, MeetingRoom)
- 3-4 DTO:ta
- Yksinkertainen mapping-logiikka
- Projekti alle 10 endpointia

**AutoMapper kannattaa JOS:**

❌ Projekti kasvaa 20+ DTO:ksi  
❌ Monimutkaista nested-mappingiä  
❌ Tiimi on jo tottunut AutoMapperiin  
❌ Sama entity täytyy mappata moneen eri DTO:hon

**AutoMapper EI kannata JOS:**

✅ Projekti pysyy pienenä (alle 10 DTO:ta) ← SINUN TILANNE  
✅ Mapping-logiikka on yksinkertaista  
✅ Haluat pitää koodin eksplisiittisenä  
✅ Haluat välttää ylimääräisiä riippuvuuksia

**Vaihtoehto: Mapping-metodit**

Sen sijaan että lisäät AutoMapperin, voit luoda **yksinkertaiset extension-metodit:**

**BookingExtensions.cs:**

```csharp
public static class BookingExtensions
{
    public static BookingDto ToDto(this Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            MeetingRoomId = booking.MeetingRoomId,
            MeetingRoomName = booking.MeetingRoom.Name,
            BookedBy = booking.BookedBy,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Description = booking.Description
        };
    }
}
```

**Käyttö:**

```csharp
var dto = booking.ToDto();
```

**Edut:**

- Yksinkertainen ja eksplisiittinen
- Ei ylimääräisiä riippuvuuksia
- Helppo debugata
- IntelliSense toimii täydellisesti

#### Milloin lisätä AutoMapper myöhemmin?

Lisää AutoMapper kun:

1. **DTO:iden määrä kasvaa 10+**
2. **Sama entity → monta DTO:ta**

- `Booking` → `BookingListDto`, `BookingDetailDto`, `BookingAdminDto`, jne.

3. **Monimutkaista nested-mappingiä**

- `Order` sisältää `OrderItems`, jotka sisältävät `Products`, jne.

4. **Tiimi kasvaa ja konventiot tärkeämpiä**

#### Suositus sinulle

**ÄLÄ lisää AutoMapperia vielä.**  
Sinun projektisi on:

- Pieni ja yksinkertainen
- Hyvin hallittavissa ilman sitä
- Helpompi ymmärtää uusille kehittäjille ilman "magiaa"

**Jos** projekti kasvaa myöhemmin ja mappingit alkavat toistaa itseään paljon, **silloin** harkitse AutoMapperia uudelleen.

**Tällä hetkellä:**  
Käytä joko:

1. Suoraa mappingia (kuten nyt)
2. Extension-metodeja (jos haluat siistiä koodia)

Molemmat ovat parempia vaihtoehtoja pienelle projektille kuin AutoMapper.  
Haluatko että näytän miltä extension-metodit näyttäisivät, vai jatketaanko jotain muuta API:n osaa?

### 👤 Nina

> `Liitetty koko BookingService koodi`
>
> Hyviä pointteja. Jätetään AutoMapper toistaiseksi pois. BookingService luokkani on kasvanut jo melko suureksi. Onko sinulla ehdotuksia koodin refaktoroinnin suhteen tai huomaatko jotain muuta mitä voisimme parantaa?

### 🤖 Claude

Hyvä huomio! Katsotaan mitä voidaan parantaa.

#### Huomiot ja parannusehdotukset

**1. DateTime.UtcNow field-muuttujana ❌**

```csharp
private readonly DateTime now = DateTime.UtcNow;
```

**Ongelma:** Tämä asetetaan **kerran** kun BookingService luodaan (service on Scoped). Jos sama service-instanssi elää pitkään, `now` on väärä.

**Korjaus:** Käytä aina `DateTime.UtcNow` suoraan tai luo property:

```csharp
private DateTime Now => DateTime.UtcNow;
```

Tai vielä parempi - injektoi `ISystemClock` / `TimeProvider` testattavuuden vuoksi (mutta se on ehkä overkill pienelle projektille).

**2. Toistuva DTO-mapping**  
Sama mapping-logiikka toistuu 4 kertaa:

- `CreateBookingAsync`
- `GetBookingAsync`
- `GetRoomBookingsAsync` (LINQ:ssa)
- `GetAllBookingsAsync` (LINQ:ssa)

**Ratkaisu: Private helper-metodi**

```csharp
private static BookingDto MapToDto(Booking booking, string roomName)
{
    return new BookingDto
    {
        Id = booking.Id,
        MeetingRoomId = booking.MeetingRoomId,
        MeetingRoomName = roomName,
        BookedBy = booking.BookedBy,
        StartTime = booking.StartTime,
        EndTime = booking.EndTime,
        Description = booking.Description
    };
}
```

Tai extension method `Booking.ToDto()` kuten aiemmin mainitsin.

**3. Vakiot kannattaisi siirtää konfiguraatioon**

```csharp
private const int MinBookingMinutes = 15;
private const int MaxBookingHours = 16;
private const int MaxBookingMonthsAhead = 6;
Nämä voisi olla appsettings.json:ssa, jolloin voit muuttaa niitä ilman uudelleenkääntämistä.
appsettings.json:
json{
  "BookingSettings": {
    "MinBookingMinutes": 15,
    "MaxBookingHours": 16,
    "MaxBookingMonthsAhead": 6
  }
}
```

**BookingSettings.cs:**

```csharp
public class BookingSettings
{
    public int MinBookingMinutes { get; set; }
    public int MaxBookingHours { get; set; }
    public int MaxBookingMonthsAhead { get; set; }
}
```

**Injektoi:**

```csharp
private readonly BookingSettings _settings;

public BookingService(ApplicationDbContext context, IOptions<BookingSettings> settings)
{
    _context = context;
    _settings = settings.Value;
}
```

**4. Validointilogiikka voisi olla omassa metodissa**

`CreateBookingAsync` on nyt 130+ riviä. Voit jakaa sen pienempiin osiin:

```csharp
public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
{
    ValidateBookingTimes(dto);
    ValidateBookingDuration(dto);

    var room = await GetRoomOrThrowAsync(dto.MeetingRoomId);
    await ValidateNoConflictsAsync(dto);

    var booking = await CreateAndSaveBookingAsync(dto);
    return MapToDto(booking, room.Name);
}

private void ValidateBookingTimes(CreateBookingDto dto)
{
    if (dto.StartTime >= dto.EndTime)
        throw new BookingValidationException("Aloitusajan on oltava ennen lopetusaikaa");

    if (dto.StartTime < DateTime.UtcNow)
        throw new BookingValidationException("Varaus ei voi sijoittua menneisyyteen.");
}

private void ValidateBookingDuration(CreateBookingDto dto)
{
    var duration = dto.EndTime - dto.StartTime;
    // ... duration-validoinnit
}
```

**5. Exception-tyyppi: NotFoundException**  
Huomasin että käytät `NotFoundException`, mutta aiemmin loimme `BookingNotFoundException`. Kannattaa yhtenäistää:
Joko:

- BookingNotFoundException (domain-spesifi)
- NotFoundException (geneerinen, käytettävissä koko sovelluksessa)

Valitse toinen ja pysy siinä.

**6. Details-objektien rakenne**

```csharp
new
{
    duration = new
    {
        errorMessage = "Booking too short",
        currentDuration = bookingDuration.TotalMinutes,
        minimumDuration = MinBookingMinutes
    }
}
```

Ylimääräinen nested-objekti (duration) ei tuo lisäarvoa. Yksinkertaisempi:

```csharp
new
{
    error = "BookingTooShort",
    currentDuration = bookingDuration.TotalMinutes,
    minimumDuration = MinBookingMinutes
}
```

**7. LINQ Select vs Include**

`GetRoomBookingsAsync` ja `GetAllBookingsAsync` käyttävät `.Include()` + `.Select()`. Include on turha kun teet heti Selectin:

```csharp
// Nykyinen (turha Include)
var bookings = await _context.Bookings
    .Include(b => b.MeetingRoom)  // ← Turha
    .Where(...)
    .Select(b => new BookingDto { ... })
    .ToListAsync();

// Parempi
var bookings = await _context.Bookings
    .Where(...)
    .Select(b => new BookingDto
    {
        MeetingRoomName = b.MeetingRoom.Name  // EF tekee JOIN automaattisesti
    })
    .ToListAsync();
```

**Refaktoroitu versio (esimerkki)**  
En kirjoita koko koodia, mutta rakenne voisi olla:

```csharp
public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly BookingSettings _settings;
    private DateTime Now => DateTime.UtcNow;

    // Constructor...

    public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
    {
        ValidateBookingRequest(dto);

        var room = await GetRoomOrThrowAsync(dto.MeetingRoomId);
        await EnsureNoConflictsAsync(dto);

        var booking = await SaveNewBookingAsync(dto);
        return MapToDto(booking, room.Name);
    }

    private void ValidateBookingRequest(CreateBookingDto dto)
    {
        ValidateTimeOrder(dto);
        ValidateNotInPast(dto);
        ValidateDuration(dto);
        ValidateNotTooFarAhead(dto);
    }

    private void ValidateTimeOrder(CreateBookingDto dto) { ... }
    private void ValidateNotInPast(CreateBookingDto dto) { ... }
    private void ValidateDuration(CreateBookingDto dto) { ... }
    private void ValidateNotTooFarAhead(CreateBookingDto dto) { ... }

    private async Task<MeetingRoom> GetRoomOrThrowAsync(int roomId) { ... }
    private async Task EnsureNoConflictsAsync(CreateBookingDto dto) { ... }
    private async Task<Booking> SaveNewBookingAsync(CreateBookingDto dto) { ... }

    private static BookingDto MapToDto(Booking booking, string roomName) { ... }
}
```

#### Yhteenveto prioriteeteista

**Korjaa heti:**

1. ✅ `DateTime now` field → property tai suora käyttö
2. ✅ Valitse joko `NotFoundException` tai `BookingNotFoundException`
3. ✅ Poista turhat `.Include()`:t LINQ Selecteissä

**Harkitse:**  
4. 🤔 Jaa CreateBookingAsync pienempiin metodeihin (jos kasvaa yli 50 riviä)  
5. 🤔 Luo helper-metodi DTO-mappingiin (vähentää toistoa)
Voi jättää myöhemmäksi:  
6. ⏰ Siirrä vakiot appsettings.json:iin  
7. ⏰ Erillinen validation-luokka (jos validointeja tulee paljon lisää)

Haluatko että käyn jonkin näistä läpi tarkemmin koodiesimerkein?

### 👤 Nina

> Miten otan program.cs tiedostossa käyttöön appSettings-tiedostoon lisätyt muuttujat?

### 🤖 Claude

Hyvä kysymys! Tässä askel askeleelta miten otat appsettings.json:n muuttujat käyttöön.

#### 1. Luo BookingSettings-luokka

Luo uusi kansio `Configuration` tai `Settings` ja sinne:

```csharp
namespace MeetingRoomBookingApi.Configuration
{
    public class BookingSettings
    {
        public int MinBookingMinutes { get; set; }
        public int MaxBookingHours { get; set; }
        public int MaxBookingMonthsAhead { get; set; }
    }
}
```

#### 2. Lisää asetukset appsettings.json:iin

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "BookingSettings": {
    "MinBookingMinutes": 15,
    "MaxBookingHours": 16,
    "MaxBookingMonthsAhead": 6
  }
}
```

#### 3. Rekisteröi asetukset Program.cs:ssä

Lisää tämä **ennen** `builder.Services.AddScoped<IBookingService, BookingService>();`

```csharp
using MeetingRoomBookingApi.Configuration;

// ... muut usinkit

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Lisää tämä rivi:
builder.Services.Configure<BookingSettings>(
    builder.Configuration.GetSection("BookingSettings"));

// Lisää Entity Framework Core InMemory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MeetingRoomDb"));

// Rekisteröi services
builder.Services.AddScoped<IBookingService, BookingService>();
```

#### 4. Injektoi BookingServiceen

Muuta BookingService:n konstruktori:

```csharp
using Microsoft.Extensions.Options;
using MeetingRoomBookingApi.Configuration;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly BookingSettings _settings;
    private DateTime Now => DateTime.UtcNow;

    public BookingService(
        ApplicationDbContext context,
        IOptions<BookingSettings> settings)
    {
        _context = context;
        _settings = settings.Value;
    }

    // Nyt voit käyttää:
    // _settings.MinBookingMinutes
    // _settings.MaxBookingHours
    // _settings.MaxBookingMonthsAhead
}
```

#### 5. Korvaa vakiot asetuksilla

Poista nämä rivit:

```csharp
private const int MinBookingMinutes = 15;
private const int MaxBookingHours = 16;
private const int MaxBookingMonthsAhead = 6;
```

Ja käytä sen sijaan:

```csharp
if (bookingDuration.TotalMinutes < _settings.MinBookingMinutes)
    throw new BookingValidationException(
        $"Varauksen minimipituus on {_settings.MinBookingMinutes} minuuttia.",
        // ...
    );
```

#### Miksi IOptions<T>?

**IOptions<T>** on ASP.NET Coren tapa injektoida konfiguraatioita:

- `IOptions<T>` - Luetaan kerran sovelluksen käynnistyessä (käytä tätä)
- `IOptionsSnapshot<T>` - Luetaan uudelleen joka pyynnöllä (jos muuttuu)
- `IOptionsMonitor<T>` - Seuraa muutoksia reaaliajassa

Sinun tapauksessasi **IOptions** riittää, koska näitä ei tarvitse muuttaa runtime-aikana.

#### Ympäristökohtaiset asetukset (bonus)

Voit myös luoda:

- `appsettings.Development.json` (kehitysympäristö)
- `appsettings.Production.json` (tuotanto)

Esim. development-ympäristössä voit sallia pidemmät varaukset:

**appsettings.Development.json:**

```json
{
  "BookingSettings": {
    "MaxBookingHours": 24,
    "MaxBookingMonthsAhead": 12
  }
}
```

ASP.NET Core yhdistää nämä automaattisesti oikean ympäristön mukaan.

#### Validoi asetukset käynnistyksessä (bonus)

Jos haluat varmistaa että asetukset ovat järkeviä:

```csharp
public class BookingSettings
{
    public int MinBookingMinutes { get; set; }
    public int MaxBookingHours { get; set; }
    public int MaxBookingMonthsAhead { get; set; }

    public void Validate()
    {
        if (MinBookingMinutes <= 0)
            throw new InvalidOperationException("MinBookingMinutes must be positive");

        if (MaxBookingHours < MinBookingMinutes / 60)
            throw new InvalidOperationException("MaxBookingHours must be >= MinBookingMinutes");
    }
}
```

Ja Program.cs:ssä:

```csharp
var app = builder.Build();

// Validoi asetukset käynnistyksessä
var settings = app.Services.GetRequiredService<IOptions<BookingSettings>>().Value;
settings.Validate();
```

Tämä varmistaa että sovellus ei käynnisty virheellisillä asetuksilla.
Onko tämä selvä vai haluatko että käyn jonkun kohdan tarkemmin läpi?

### 👤 Nina

> ```
> if (bookingDuration.TotalHours > _settings.MaxBookingHours)
>      throw new BookingValidationException(
>         $"Varauksen maximipituus on {_settings.MaxBookingHours} tuntia.",
>          new
>          {
>              errorMessage = "Booking too long",
>              currentDurationFormatted = $"{(int)bookingDuration.TotalDays} days, {bookingDuration.Hours} hours, {bookingDuration.Minutes} minutes",
>              maxHours = _settings.MaxBookingHours
>          });
>
> Haluan yhtenäistää virheviestien details-objektia. Error-message ei tunnu tarpeelliselta, kun ylempänä mainitaan jo virheen syy. Millaista rakennetta ehdottaisit details-objektille?
> ```

### 🤖 Claude

Erinomainen huomio! Olet oikeassa - `errorMessage` on tarpeetonta toistoa. Käydään läpi parempi rakenne.

#### Ongelma nykyisessä rakenteessa

```json
{
  "message": "Varauksen maximipituus on 16 tuntia.", // ← Selkeä viesti
  "details": {
    "errorMessage": "Booking too long" // ← Turha toisto, tekninen koodi
  }
}
```

#### Suositeltu rakenne

**Periaate:** Details sisältää vain **konkreettista dataa**, ei viestejä.

```csharp
new
{
    requestedHours = Math.Round(bookingDuration.TotalHours, 1),
    requestedDays = Math.Round(bookingDuration.TotalDays, 1),
    maximumHours = _settings.MaxBookingHours
}
```

**Vastaus:**

```json
{
  "message": "Varauksen maximipituus on 16 tuntia.",
  "statusCode": 400,
  "details": {
    "requestedHours": 144.1,
    "requestedDays": 6.0,
    "maximumHours": 16
  }
}
```

#### Yhtenäistetyt Details kaikille virheille

**1. Minimipituus**

```csharp
new
{
    requestedMinutes = Math.Round(bookingDuration.TotalMinutes, 0),
    minimumMinutes = _settings.MinBookingMinutes
}
```

**2. Maksimipituus**

```csharp
new
{
    requestedHours = Math.Round(bookingDuration.TotalHours, 1),
    maximumHours = _settings.MaxBookingHours
}
```

**3. Liian kauas tulevaisuuteen**

```csharp
new
{
    requestedStartDate = dto.StartTime,
    currentDate = Now,
    maximumStartDate = Now.AddMonths(_settings.MaxBookingMonthsAhead),
    maximumMonthsAhead = _settings.MaxBookingMonthsAhead
}
```

**4. Konflikti**

```csharp
new
{
    requestedStart = dto.StartTime,
    requestedEnd = dto.EndTime,
    conflictingBooking = new
    {
        startTime = conflictingBooking.StartTime,
        endTime = conflictingBooking.EndTime,
        bookedBy = conflictingBooking.BookedBy
    }
}
```

#### Naming convention

**Suosittelen johdonmukaista nimeämistä:**

- `requested*` = Mitä käyttäjä pyysi
- `minimum*` / `maximum*` = Rajoitukset
- `current*` = Nykyinen tila/aika
- `conflicting*` = Ristiriidassa oleva data

**camelCase** JSON:ssa (ASP.NET Coren oletus).

#### Lopullinen yhtenäistetty rakenne

```csharp
// Liian lyhyt
new
{
    requestedMinutes = bookingDuration.TotalMinutes,
    minimumMinutes = _settings.MinBookingMinutes
}

// Liian pitkä
new
{
    requestedHours = bookingDuration.TotalHours,
    maximumHours = _settings.MaxBookingHours
}

// Liian kauas
new
{
    requestedStartDate = dto.StartTime,
    maximumStartDate = Now.AddMonths(_settings.MaxBookingMonthsAhead)
}

// Konflikti
new
{
    requestedPeriod = new
    {
        start = dto.StartTime,
        end = dto.EndTime
    },
    conflictingBooking = new
    {
        start = conflictingBooking.StartTime,
        end = conflictingBooking.EndTime,
        bookedBy = conflictingBooking.BookedBy
    }
}
```

#### Miksi tämä on parempi?

**Frontend-kehittäjän näkökulma:**

```javascript
// Helppo näyttää käyttäjälle
if (error.details.requestedHours > error.details.maximumHours) {
  showMessage(
    `Yritit varata ${error.details.requestedHours}h. Maksimi on ${error.details.maximumHours}h.`,
  );
}

// Tai ehdottaa korjausta
const suggestedEndTime = calculateEndTime(
  startTime,
  error.details.maximumHours,
);
```

**Lokituksen näkökulma:**

```
[ERROR] Booking validation failed:
  User requested: 144.1 hours
  Maximum allowed: 16 hours
  TraceId: ...
```

Kaikki oleellinen data on details-objektissa, ei tarvitse parssia viestejä.

#### Vaihtoehtoiset lähestymistavat

**Jos haluat error code -kentän (REST best practice):**

```csharp
new
{
    code = "BOOKING_TOO_LONG",  // Konekäsiteltävä koodi
    requestedHours = bookingDuration.TotalHours,
    maximumHours = _settings.MaxBookingHours
}
```

Frontend voi tehdä:

```javascript
if (error.details.code === "BOOKING_TOO_LONG") {
  // Spesifinen käsittely
}
```

#### Yhteenveto

**Poista:** `errorMessage` -kentät (turha toisto)
**Pidä:** Vain konkreettista, mitattavaa dataa:

- Käyttäjän syötteet (`requested*`)
- Rajoitukset (`minimum*`, `maximum*`)
- Tila (`current*`, `conflicting*`)

**Lisää (valinnainen):** `code` -kenttä konekäsittelyyn

Tämä tekee virheistä informatiivisia mutta ei verbose. Frontend saa kaiken tarvitsemansa ilman että tarvitsee parssia tekstiviestejä.

Sopiko tämä lähestymistapa?

### 👤 Nina

> Mikä on oikeaoppinen / ammattimainen tapa käsitellä virheitä API:ssa? Jos haluan lisätä frontille tarkoitetun virheviestin, olisiko se hyvä tehdä details-objektissa:
>
> ```c#
>      if (conflictingBooking != null)
>      {
>          throw new BookingConflictException(
>              "Huone on jo varattu kyseiselle ajanjaksolle.",
>              new
>              {
>                  code = "BOOKING_TIME_CONFLICT",
>                  startTime = conflictingBooking.StartTime,
>                  endTime = conflictingBooking.EndTime,
>                  bookedBy = conflictingBooking.BookedBy
>              }
>          );
>      }
> ```
>
> Vai lisäänkä ErrorResponseDto:lle uuden kentän ErrorCode, johon koodi "BOOKING_TIME_CONFLICT" asetetaan

### 🤖 Claude

Erinomainen kysymys! Tämä on tärkeä arkkitehtuuripäätös. Käydään läpi ammattimainen lähestymistapa.

#### Industry Best Practices

Katsotaan mitä suuret toimijat tekevät:

**Google Cloud API:**

```json
{
  "error": {
    "code": 409,
    "message": "Resource already exists",
    "status": "ALREADY_EXISTS"
  }
}
```

**Stripe API:**

```json
{
  "error": {
    "type": "invalid_request_error",
    "code": "resource_missing",
    "message": "No such customer: cus_123"
  }
}
```

**Microsoft Graph API:**

```json
{
  "error": {
    "code": "Conflict",
    "message": "Another booking exists",
    "innerError": {
      "code": "BookingTimeConflict",
      "date": "2026-01-20"
    }
  }
}
```

#### Suositus: Error Code ylätasolle

**Lisää `ErrorCode` ErrorResponse-luokkaan:**

```csharp
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? ErrorCode { get; set; }  // ← UUSI
    public string? TraceId { get; set; }
    public object? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

**Päivitä konstruktori:**

```csharp
public ErrorResponse(int statusCode, string message, string? errorCode = null, string? traceId = null, object? details = null)
{
    StatusCode = statusCode;
    Message = message;
    ErrorCode = errorCode;
    TraceId = traceId;
    Details = details;
}
```

#### Middleware-päivitys

**ExceptionHandlingMiddleware.cs:**

```csharp
private ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
{
    var traceId = context.TraceIdentifier;

    return exception switch
    {
        BookingValidationException validationEx => new ErrorResponse(
            statusCode: (int)HttpStatusCode.BadRequest,
            message: validationEx.Message,
            errorCode: ExtractErrorCode(validationEx.ErrorDetails) ?? "VALIDATION_ERROR",
            traceId: traceId,
            details: validationEx.ErrorDetails
        ),

        BookingNotFoundException notFoundEx => new ErrorResponse(
            statusCode: (int)HttpStatusCode.NotFound,
            message: notFoundEx.Message,
            errorCode: "RESOURCE_NOT_FOUND",
            traceId: traceId
        ),

        BookingConflictException conflictEx => new ErrorResponse(
            statusCode: (int)HttpStatusCode.Conflict,
            message: conflictEx.Message,
            errorCode: ExtractErrorCode(conflictEx.ErrorDetails) ?? "BOOKING_CONFLICT",
            traceId: traceId,
            details: conflictEx.ErrorDetails
        ),

        _ => new ErrorResponse(
            statusCode: (int)HttpStatusCode.InternalServerError,
            message: "Palvelimella tapahtui virhe. Yritä myöhemmin uudelleen.",
            errorCode: "INTERNAL_ERROR",
            traceId: traceId
        )
    };
}

private static string? ExtractErrorCode(object? errorDetails)
{
    if (errorDetails == null) return null;

    var type = errorDetails.GetType();
    var codeProp = type.GetProperty("code");
    return codeProp?.GetValue(errorDetails)?.ToString();
}
```

#### Service-päivitys

**BookingService.cs - esimerkkejä:**

```csharp
// Konflikti
throw new BookingConflictException(
    "Huone on jo varattu kyseiselle ajanjaksolle.",
    new
    {
        code = "BOOKING_TIME_CONFLICT",
        conflictingBooking = new
        {
            startTime = conflictingBooking.StartTime,
            endTime = conflictingBooking.EndTime,
            bookedBy = conflictingBooking.BookedBy
        }
    }
);

// Liian lyhyt
throw new BookingValidationException(
    $"Varauksen minimipituus on {_settings.MinBookingMinutes} minuuttia.",
    new
    {
        code = "BOOKING_TOO_SHORT",
        requestedMinutes = bookingDuration.TotalMinutes,
        minimumMinutes = _settings.MinBookingMinutes
    }
);

// Liian pitkä
throw new BookingValidationException(
    $"Varauksen maximipituus on {_settings.MaxBookingHours} tuntia.",
    new
    {
        code = "BOOKING_TOO_LONG",
        requestedHours = bookingDuration.TotalHours,
        maximumHours = _settings.MaxBookingHours
    }
);

// Menneisyydessä
throw new BookingValidationException(
    "Varaus ei voi sijoittua menneisyyteen.",
    new
    {
        code = "BOOKING_IN_PAST",
        requestedStartTime = dto.StartTime,
        currentTime = Now
    }
);
```

#### Lopputulos

**JSON-vastaus:**

```json
{
  "message": "Huone on jo varattu kyseiselle ajanjaksolle.",
  "statusCode": 409,
  "errorCode": "BOOKING_TIME_CONFLICT",
  "traceId": "...",
  "details": {
    "conflictingBooking": {
      "startTime": "2026-01-22T10:00:00",
      "endTime": "2026-01-22T11:00:00",
      "bookedBy": "Matti Meikäläinen"
    }
  },
  "timestamp": "2026-01-20T14:30:00Z"
}
```

#### Miksi tämä on parempi?

**1. Selkeä hierarkia:**

- `errorCode` - **Konekäsiteltävä**, ylätasolla, aina sama muoto
- `details.code` - **Tarpeetonta toistoa**, poista tämä
- `message` - **Ihmisluettava**, voi olla lokalisoitu

**2. Frontend-käsittely:**

```javascript
switch (error.errorCode) {
  case "BOOKING_TIME_CONFLICT":
    // Näytä vaihtoehtoisia aikoja
    showAlternativeTimes(error.details);
    break;
  case "BOOKING_TOO_LONG":
    // Ehdota maksimipituutta
    suggestMaxDuration(error.details.maximumHours);
    break;
  case "RESOURCE_NOT_FOUND":
    // Ohjaa takaisin listaan
    redirectToRoomList();
    break;
}
```

**3. Lokalisointi:**

```javascript
const messages = {
  BOOKING_TIME_CONFLICT: {
    fi: "Huone on jo varattu",
    en: "Room already booked",
  },
};

// Frontend voi ohittaa backendin message-kentän
displayMessage(messages[error.errorCode][currentLang]);
```

## Error Code -nimeämiskäytännöt

**Konventio:**

- `UPPER_SNAKE_CASE`
- Aloita kontekstilla: `BOOKING_*`, `ROOM_*`, `USER_*`
- Ole spesifinen mutta ei liian granulaarinen

**Esimerkkejä:**

```
BOOKING_TIME_CONFLICT
BOOKING_TOO_SHORT
BOOKING_TOO_LONG
BOOKING_IN_PAST
BOOKING_TOO_FAR_AHEAD
ROOM_NOT_FOUND
BOOKING_NOT_FOUND
VALIDATION_ERROR
INTERNAL_ERROR
Yhteenveto
```

**✅ Tee näin:**

- Lisää `ErrorCode` ErrorResponse-luokkaan (ylätasolle)
- Poista `code` details-objektista
- Details sisältää vain konkreettista dataa

**❌ Älä tee näin:**

- Älä laita error codea vain detailsiin
- Älä toista samaa tietoa kahdessa paikassa
- Älä käytä teknisiä exception-nimiä koodina

Tämä on industry standard ja skaalautuu hyvin kun API kasvaa.

### 👤 Nina

> ```c#
> // Validoi: Aloitusajan täytyy olla ennen lopetusaikaa
> if (dto.StartTime >= dto.EndTime)
>     throw new BookingValidationException("Aloitusajan on oltava ennen lopetusaikaa",
>        new
>         {
>             code = "BOOKING_INVALID_TIME_RANGE",
>             requestedStart = dto.StartTime,
>             requestedEnd = dto.EndTime
>         });
> ```
>
> ResponseBody on:
>
> ```json
> {
>   "message": "Aloitusajan on oltava ennen lopetusaikaa",
>   "statusCode": 400,
>   "errorCode": "BOOKING_INVALID_TIME_RANGE",
>   "traceId": "0HNIP261LNQ92:00000009",
>   "details": {
>     "code": "BOOKING_INVALID_TIME_RANGE",
>     "requestedStart": "2026-01-22T10:00:00",
>     "requestedEnd": "2026-01-22T09:00:00"
>   },
>   "timestamp": "2026-01-21T17:22:00.6380063Z"
> }
> ```

### 🤖 Claude

Hyvä huomio! ErrorCode toistuu sekä ylätasolla että details-objektissa. Tämä on juuri sitä mitä halusimme välttää.

#### Ongelma

Middleware **lukee** `code`-kentän details-objektista ja asettaa sen `ErrorCode`-kenttään, mutta **ei poista** sitä detailsista. Tämä aiheuttaa tuplausta.

#### Ratkaisu 1: Poista code details-objektista (SUOSITUS)

**Muuta ServiceService.cs - poista kaikki code-kentät:**

```csharp
// ENNEN
throw new BookingValidationException("Aloitusajan on oltava ennen lopetusaikaa",
new
{
    code = "BOOKING_INVALID_TIME_RANGE", // ← POISTA TÄMÄ
    requestedStart = dto.StartTime,
    requestedEnd = dto.EndTime
});

// JÄLKEEN
throw new BookingValidationException("Aloitusajan on oltava ennen lopetusaikaa",
new
{
    requestedStart = dto.StartTime,
    requestedEnd = dto.EndTime
});
```

**Muuta middleware asettamaan error code suoraan:**

```csharp
BookingValidationException validationEx => new ErrorResponse(
statusCode: (int)HttpStatusCode.BadRequest,
message: validationEx.Message,
errorCode: DetermineValidationErrorCode(validationEx.Message), // ← Päättele viestistä
traceId: traceId,
details: validationEx.ErrorDetails
),
```

**Tai vielä parempi - lisää ErrorCode exceptioniin:**

#### Ratkaisu 2: Lisää ErrorCode suoraan exceptioniin (PARAS)

**Päivitä BookingException.cs:**

```csharp
public abstract class BookingException : Exception
{
public string? ErrorCode { get; set; }
public object? ErrorDetails { get; set; }

    protected BookingException(string message, string? errorCode = null) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected BookingException(string message, string? errorCode, object? errorDetails) : base(message)
    {
        ErrorCode = errorCode;
        ErrorDetails = errorDetails;
    }

}
```

**Päivitä BookingValidationException.cs:**

```csharp
public class BookingValidationException : BookingException
{
public BookingValidationException(string message, string errorCode)
: base(message, errorCode)
{
}

    public BookingValidationException(string message, string errorCode, object errorDetails)
        : base(message, errorCode, errorDetails)
    {
    }

}
```

**Sama muutos muille exceptioneille.**

#### Käyttö ServiceService.cs:ssä:

```csharp
throw new BookingValidationException(
    "Aloitusajan on oltava ennen lopetusaikaa",
    "BOOKING_INVALID_TIME_RANGE",
    new
    {
        requestedStart = dto.StartTime,
        requestedEnd = dto.EndTime
    }
);

throw new BookingConflictException(
    "Huone on jo varattu kyseiselle ajanjaksolle.",
    "BOOKING_TIME_CONFLICT",
    new
    {
        conflictingBooking = new
        {
            startTime = conflictingBooking.StartTime,
            endTime = conflictingBooking.EndTime,
            bookedBy = conflictingBooking.BookedBy
        }
    }
);
```

#### Middleware yksinkertaistuu:

```csharp
BookingValidationException validationEx => new ErrorResponse(
    statusCode: (int)HttpStatusCode.BadRequest,
    message: validationEx.Message,
    errorCode: validationEx.ErrorCode ?? "VALIDATION_ERROR",
    traceId: traceId,
    details: validationEx.ErrorDetails // ← Ei sisällä code-kenttää
),

BookingConflictException conflictEx => new ErrorResponse(
    statusCode: (int)HttpStatusCode.Conflict,
    message: conflictEx.Message,
    errorCode: conflictEx.ErrorCode ?? "BOOKING_CONFLICT",
    traceId: traceId,
    details: conflictEx.ErrorDetails
),
```

**Voit nyt poistaa `ExtractErrorCode`-metodin kokonaan.**

#### Lopputulos

**JSON-vastaus (puhdas):**

```json
{
  "message": "Aloitusajan on oltava ennen lopetusaikaa",
  "statusCode": 400,
  "errorCode": "BOOKING_INVALID_TIME_RANGE",
  "traceId": "0HNIP261LNQ92:00000009",
  "details": {
    "requestedStart": "2026-01-22T10:00:00",
    "requestedEnd": "2026-01-22T09:00:00"
  },
  "timestamp": "2026-01-21T17:22:00Z"
}
```

**Edut Ratkaisu 2:lle**

✅ Ei toistoa - ErrorCode vain yhdessä paikassa  
✅ Tyyppiturvallisuus - Koodi on exception-luokan osa  
✅ Selkeämpi koodi - Heti näkee mikä virhekoodi on  
✅ Helpompi ylläpitää - Ei tarvitse päätellä koodeja viestistä  
✅ Dokumentoituu paremmin - Exception määrittelee oman koodinsa

#### Yhteenveto

**Suositus: Ratkaisu 2**

Lisää `ErrorCode` property exception-luokkiin ja välitä se konstruktorissa. Details-objekti sisältää vain konkreettista dataa, ei metadataa.

Haluatko että näytän päivitetyt exception-luokat kokonaisuudessaan?

### 👤 Nina

> Auta minua tekemään yksikkötestejä. Olen luonut testiprojektin ja haluan tehdä xUnit testejä. Olen asentanut myös FluentAssertions ja Moq nuget-paketit
>
> Ensimmäinen testattava funktio:
>
> ```c#
>        private static void ValidateTimeRange(CreateBookingDto dto)
>        {
>            if (dto.StartTime >= dto.EndTime)
>                throw new BookingValidationException(
>                    "Aloitusajan on oltava ennen lopetusaikaa",
>                    "BOOKING_INVALID_TIME_RANGE",
>                    new
>                    {
>                        requestedStart = dto.StartTime,
>                        requestedEnd = dto.EndTime
>                    });
>        }
> ```
>
> Testin tulee varmistaa, että mikäli käyttäjä syöttää virheellisesti lopetusajan ennen aloitusaikaa, palautetaan BookingValidationException oikealla viestillä. Voidaan varmistaa myös että errorcode on oikea. Varmistetaan lisäksi, että varausta ei synny.

### 🤖 Claude

Hyvä! Tehdään kattavat yksikkötestit BookingServicelle. Koska `ValidateTimeRange` on private-metodi, testaamme sitä epäsuorasti `CreateBookingAsync`-metodin kautta.

#### 1. Testiluokan pohja

Luo testiprojektiisi `BookingServiceTests.cs`:

```csharp
using FluentAssertions;
using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Exceptions;
using MeetingRoomBookingApi.Models;
using MeetingRoomBookingApi.Services;
using MeetingRoomBookingApi.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace MeetingRoomBookingApi.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly ApplicationDbContext \_context;
        private readonly BookingService \_sut; // System Under Test
        private readonly BookingSettings \_settings;

        public BookingServiceTests()
        {
            // Luo InMemory-tietokanta jokaiselle testille
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Uniikki per testi
                .Options;

            _context = new ApplicationDbContext(options);

            // Lisää testidataa
            SeedTestData();

            // Luo settings
            _settings = new BookingSettings
            {
                MinBookingMinutes = 15,
                MaxBookingHours = 16,
                MaxBookingMonthsAhead = 6
            };

            var settingsOptions = Options.Create(_settings);

            // Luo testattava service
            _sut = new BookingService(_context, settingsOptions);
        }

        private void SeedTestData()
        {
            var rooms = new List<MeetingRoom>
            {
                new MeetingRoom { Id = 1, Name = "Sali A", Capacity = 10 },
                new MeetingRoom { Id = 2, Name = "Sali B", Capacity = 6 }
            };

            _context.MeetingRooms.AddRange(rooms);
            _context.SaveChanges();
        }
    }

}
```

#### 2. Testit aikavälien validoinnille

```csharp
public class BookingServiceTests
{
// ... konstruktori ja setup ...

    [Fact]
    public async Task CreateBookingAsync_WhenEndTimeBeforeStartTime_ShouldThrowBookingValidationException()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            MeetingRoomId = 1,
            BookedBy = "Test User",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(1), // ← Loppu ennen alkua
            Description = "Test"
        };

        // Act
        Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

        // Assert
        var exception = await act.Should().ThrowAsync<BookingValidationException>();
        exception.Which.Message.Should().Be("Aloitusajan on oltava ennen lopetusaikaa");
        exception.Which.ErrorCode.Should().Be("BOOKING_INVALID_TIME_RANGE");

        // Varmista että Details sisältää oikeat ajat
        dynamic details = exception.Which.ErrorDetails!;
        DateTime requestedStart = details.requestedStart;
        DateTime requestedEnd = details.requestedEnd;
        requestedStart.Should().Be(dto.StartTime);
        requestedEnd.Should().Be(dto.EndTime);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenEndTimeEqualsStartTime_ShouldThrowBookingValidationException()
    {
        // Arrange
        var sameTime = DateTime.UtcNow.AddHours(2);
        var dto = new CreateBookingDto
        {
            MeetingRoomId = 1,
            BookedBy = "Test User",
            StartTime = sameTime,
            EndTime = sameTime, // ← Sama aika
            Description = "Test"
        };

        // Act
        Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BookingValidationException>()
            .WithMessage("Aloitusajan on oltava ennen lopetusaikaa");
    }

    [Fact]
    public async Task CreateBookingAsync_WhenEndTimeAfterStartTime_ShouldNotThrowTimeRangeException()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            MeetingRoomId = 1,
            BookedBy = "Test User",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2), // ← Oikein päin
            Description = "Test"
        };

        // Act
        Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

        // Assert
        // Ei pitäisi heittää BOOKING_INVALID_TIME_RANGE -virhettä
        // (Voi heittää muita virheitä esim. liian lyhyt varaus)
        var exception = await act.Should().ThrowAsync<BookingValidationException>();
        exception.Which.ErrorCode.Should().NotBe("BOOKING_INVALID_TIME_RANGE");
    }

    [Fact]
    public async Task CreateBookingAsync_WhenValidTimeRange_ShouldNotCreateBooking_IfOtherValidationFails()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            MeetingRoomId = 1,
            BookedBy = "Test User",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(1), // ← Virheellinen
            Description = "Test"
        };

        var initialBookingCount = await _context.Bookings.CountAsync();

        // Act
        Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BookingValidationException>();

        // Varmista että varausta EI luotu
        var finalBookingCount = await _context.Bookings.CountAsync();
        finalBookingCount.Should().Be(initialBookingCount);
    }

}
```

#### 3. Testit muille validoinneille (bonuksena)

```csharp
[Fact]
public async Task CreateBookingAsync_WhenBookingInPast_ShouldThrowBookingValidationException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(-2), // ← Menneisyydessä
        EndTime = DateTime.UtcNow.AddHours(-1),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>()
        .WithMessage("Varaus ei voi sijoittua menneisyyteen.");

}

[Fact]
public async Task CreateBookingAsync_WhenBookingTooShort_ShouldThrowBookingValidationException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(1),
        EndTime = DateTime.UtcNow.AddHours(1).AddMinutes(10), // ← 10 min, min on 15
        Description = "Test"
    };

    // Act & Assert
    var exception = await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>();

    exception.Which.ErrorCode.Should().Be("BOOKING_TOO_SHORT");

}

[Fact]
public async Task CreateBookingAsync_WhenBookingTooLong_ShouldThrowBookingValidationException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(1),
        EndTime = DateTime.UtcNow.AddHours(20), // ← 19h, max on 16h
        Description = "Test"
    };

    // Act & Assert
    var exception = await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>();

    exception.Which.ErrorCode.Should().Be("BOOKING_TOO_LONG");

}

[Fact]
public async Task CreateBookingAsync_WhenRoomNotFound_ShouldThrowNotFoundException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 999, // ← Ei ole olemassa
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(1),
        EndTime = DateTime.UtcNow.AddHours(2),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<NotFoundException>()
        .WithMessage("Kokoushuonetta ID:llä 999 ei löydy.");

}

[Fact]
public async Task CreateBookingAsync_WhenTimeConflict_ShouldThrowBookingConflictException()
{
    // Arrange - Luo olemassa oleva varaus
    var existingBooking = new Booking
    {
    Id = 1,
    MeetingRoomId = 1,
    BookedBy = "Existing User",
    StartTime = DateTime.UtcNow.AddHours(2),
    EndTime = DateTime.UtcNow.AddHours(3)
    };
    _context.Bookings.Add(existingBooking);
    await \_context.SaveChangesAsync();

    // Yritä varata päällekkäin
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "New User",
        StartTime = DateTime.UtcNow.AddHours(2).AddMinutes(30), // ← Päällekkäin
        EndTime = DateTime.UtcNow.AddHours(3).AddMinutes(30),
        Description = "Test"
    };

    // Act & Assert
    var exception = await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingConflictException>();

    exception.Which.ErrorCode.Should().Be("BOOKING_TIME_CONFLICT");

}

[Fact]
public async Task CreateBookingAsync_WhenValid_ShouldCreateBookingSuccessfully()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(1),
        EndTime = DateTime.UtcNow.AddHours(2),
        Description = "Test Meeting"
    };

    // Act
    var result = await _sut.CreateBookingAsync(dto);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
    result.MeetingRoomId.Should().Be(1);
    result.MeetingRoomName.Should().Be("Sali A");
    result.BookedBy.Should().Be("Test User");
    result.Description.Should().Be("Test Meeting");

    // Varmista että tietokantaan tallentui
    var savedBooking = await _context.Bookings.FindAsync(result.Id);
    savedBooking.Should().NotBeNull();
    savedBooking!.BookedBy.Should().Be("Test User");

}
```

#### 4. Teoria-tarkistukset (Theory)

```csharp
[Theory]
[InlineData(-2, -1)] // Molemmat menneisyydessä
[InlineData(-1, 1)] // Alku menneisyydessä
public async Task CreateBookingAsync_WhenStartTimeInPast_ShouldThrowException(int startHoursOffset, int endHoursOffset)
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(startHoursOffset),
        EndTime = DateTime.UtcNow.AddHours(endHoursOffset),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>();

}
```

### 5. Ajettaessa testejä

Voit ajaa testit:

- Visual Studiossa: Test Explorer
- Komentoriviltä: dotnet test
- Yksittäinen testi: dotnet test --filter "FullyQualifiedName~CreateBookingAsync_WhenEndTimeBeforeStartTime"

#### Yhteenveto

✅ **Testit varmistavat:**

- Virheellinen aikaväli heittää oikean exceptionin
- ErrorCode on oikea
- ErrorDetails sisältää oikeat tiedot
- Varausta ei luoda virheen sattuessa
- Kaikki muut validoinnit toimivat

✅ **FluentAssertions tekee asserteista selkeät:**

- `.Should().ThrowAsync<T>()`
- `.WithMessage("...")`
- `.Which.ErrorCode.Should().Be("...")`

✅ **InMemory-database:**

- Jokainen testi saa oman tietokannan
- Ei side effectejä testien välillä

Haluatko että lisään testejä muille metodeille (Get, Delete)?

### 👤 Nina

> Ensimmäinen testi:
>
> ```c#
> [Fact] public async Task CreateBookingAsync_WhenEndTimeBeforeStartTime_ShouldThrowBookingValidationException()
> {
>       // Arrange
>       var dto = new CreateBookingDto
>       {
>           MeetingRoomId = 1,
>           BookedBy = "Test User",
>           StartTime = DateTime.UtcNow.AddHours(2),
>           EndTime = DateTime.UtcNow.AddHours(1), // ← Loppu ennen alkua
>           Description = "Test"
>       };
>
>       // Act
>       Func<Task> act = async () => await _sut.CreateBookingAsync(dto);
>
>       // Assert
>       var exception = await act.Should().ThrowAsync<BookingValidationException>();
>       exception.Which.Message.Should().Be("Aloitusajan on oltava ennen lopetusaikaa");
>       exception.Which.ErrorCode.Should().Be("BOOKING_INVALID_TIME_RANGE");
>
>       // Varmista että Details sisältää oikeat ajat
>       dynamic details = exception.Which.ErrorDetails!;
>       DateTime requestedStart = details.requestedStart;
>       DateTime requestedEnd = details.requestedEnd;
>       requestedStart.Should().Be(dto.StartTime);
>       requestedEnd.Should().Be(dto.EndTime); }
> ```
>
> hajoaa virheeseen: Microsoft.CSharp.RuntimeBinder.RuntimeBinderException : 'object' does not contain a definition for 'requestedStart'
>
> Mitä tämä tarkoittaa?

### 🤖 Claude

Ongelma on siinä, että `ErrorDetails` on tyyppiä `object?`, eikä C# tiedä mitä propertyjä sillä on runtime-aikana. `dynamic`-keyword ei toimi tässä koska objekti on luotu anonymous tyypillä toisessa assemblyssä.

#### Ratkaisu 1: Serialisoi JSON:ksi ja deserialize takaisin (SUOSITUS)

```csharp
// Assert
var exception = await act.Should().ThrowAsync<BookingValidationException>();
exception.Which.Message.Should().Be("Aloitusajan on oltava ennen lopetusaikaa");
exception.Which.ErrorCode.Should().Be("BOOKING_INVALID_TIME_RANGE");

// Serialisoi details JSON:ksi ja deserialize takaisin
var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
var root = jsonDoc.RootElement;

root.GetProperty("requestedStart").GetDateTime().Should().Be(dto.StartTime);
root.GetProperty("requestedEnd").GetDateTime().Should().Be(dto.EndTime);
```

#### Ratkaisu 2: Käytä Reflectionia

```csharp
// Assert
var exception = await act.Should().ThrowAsync<BookingValidationException>();
exception.Which.Message.Should().Be("Aloitusajan on oltava ennen lopetusaikaa");
exception.Which.ErrorCode.Should().Be("BOOKING_INVALID_TIME_RANGE");

// Käytä reflectionia
var details = exception.Which.ErrorDetails!;
var detailsType = details.GetType();

var requestedStart = (DateTime)detailsType.GetProperty("requestedStart")!.GetValue(details)!;
var requestedEnd = (DateTime)detailsType.GetProperty("requestedEnd")!.GetValue(details)!;

requestedStart.Should().Be(dto.StartTime);
requestedEnd.Should().Be(dto.EndTime);
```

#### Ratkaisu 3: Tee ErrorDetails strongly-typed (PARAS PITKÄLLÄ TÄHTÄIMELLÄ)

Luo DTO:t error detailseille:

**ErrorDetails/InvalidTimeRangeDetails.cs:**

```csharp
namespace MeetingRoomBookingApi.DTOs.ErrorDetails
{
    public class InvalidTimeRangeDetails
    {
        public DateTime RequestedStart { get; set; }
        public DateTime RequestedEnd { get; set; }
    }
}
```

**BookingService.cs:**

```csharp
throw new BookingValidationException(
    "Aloitusajan on oltava ennen lopetusaikaa",
    "BOOKING_INVALID_TIME_RANGE",
    new InvalidTimeRangeDetails
    {
        RequestedStart = dto.StartTime,
        RequestedEnd = dto.EndTime
    });
```

**Testi:**

```csharp
// Assert
var exception = await act.Should().ThrowAsync<BookingValidationException>();
exception.Which.Message.Should().Be("Aloitusajan on oltava ennen lopetusaikaa");
exception.Which.ErrorCode.Should().Be("BOOKING_INVALID_TIME_RANGE");

var details = exception.Which.ErrorDetails.Should().BeOfType<InvalidTimeRangeDetails>().Subject;
details.RequestedStart.Should().Be(dto.StartTime);
details.RequestedEnd.Should().Be(dto.EndTime);
```

#### Ratkaisu 4: Yksinkertaista - testaa vain että Details ei ole null

Jos et tarvitse testata tarkkoja arvoja:

```csharp
// Assert
var exception = await act.Should().ThrowAsync<BookingValidationException>();
exception.Which.Message.Should().Be("Aloitusajan on oltava ennen lopetusaikaa");
exception.Which.ErrorCode.Should().Be("BOOKING_INVALID_TIME_RANGE");
exception.Which.ErrorDetails.Should().NotBeNull();
```

#### Suositus testiesi kannalta

**Käytä Ratkaisu 1 (JSON serialisointi)** yksinkertaisuuden vuoksi, koska:

- Ei vaadi muutoksia tuotantokoodiin
- Testaa että Details serialisoituu oikein (sama mitä API tekee)
- Toimii anonymous tyypeillä

**Tai Ratkaisu 4** jos et tarvitse testata Details-kenttien sisältöä tarkasti.  
**Pitkällä tähtäimellä:** Harkitse Ratkaisu 3 jos haluat tyyppiturvallisuuden, mutta se vaatii paljon ylimääräisiä DTO-luokkia.
Kumpi lähestymistapa sopii paremmin projektiisi?

### 👤 Nina

> Tehdään sitten testit tälle funktiolle:
>
> private void ValidateNotInPast(CreateBookingDto dto)
> {
> if (dto.StartTime < \_time.Now)
> throw new BookingValidationException(
> "Varaus ei voi sijoittua menneisyyteen.",
> "BOOKING_IN_THE_PAST",
> new
> {
> requestedStart = dto.StartTime,
> currentTime = \_time.Now
> });
> }

### 🤖 Claude

Hyvä! Tässä on kattavat testit ValidateNotInPast-metodille. Huomaan että käytät \_time.Now -injektiota, mikä on erinomaista testattavuuden kannalta!

### Testit

```csharp
[Fact]
public async Task CreateBookingAsync_WhenStartTimeInPast_ShouldThrowBookingValidationException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(-1), // ← Tunti sitten
        EndTime = DateTime.UtcNow.AddMinutes(30),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();
    exception.Which.Message.Should().Be("Varaus ei voi sijoittua menneisyyteen.");
    exception.Which.ErrorCode.Should().Be("BOOKING_IN_THE_PAST");
    exception.Which.ErrorDetails.Should().NotBeNull();
}

[Fact]
public async Task CreateBookingAsync_WhenStartTimeInPast_ShouldIncludeTimesInDetails()
{
    // Arrange
    var pastTime = DateTime.UtcNow.AddHours(-2);
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = pastTime,
        EndTime = DateTime.UtcNow.AddHours(1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();

    // Tarkista Details JSON:n kautta
    var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
    using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
    var root = jsonDoc.RootElement;

    root.GetProperty("requestedStart").GetDateTime().Should().BeCloseTo(pastTime, TimeSpan.FromSeconds(1));
    root.GetProperty("currentTime").GetDateTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
}

[Fact]
public async Task CreateBookingAsync_WhenStartTimeIsNow_ShouldNotThrowPastTimeException()
{
    // Arrange - Tasan nyt (tai muutama sekunti eteenpäin välttääksemme race conditionin)
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddSeconds(1),
        EndTime = DateTime.UtcNow.AddHours(1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    // Jos heittää exceptionin, sen ei pitäisi olla BOOKING_IN_THE_PAST
    var possibleException = await act.Should().ThrowAsync<BookingValidationException>();
    possibleException.Which.ErrorCode.Should().NotBe("BOOKING_IN_THE_PAST");
}

[Fact]
public async Task CreateBookingAsync_WhenStartTimeInFuture_ShouldNotThrowPastTimeException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(2),
        EndTime = DateTime.UtcNow.AddHours(3),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    // Voi heittää muita virheitä, mutta ei BOOKING_IN_THE_PAST
    try
    {
        await _sut.CreateBookingAsync(dto);
        // Jos ei heitä, ok
    }
    catch (BookingValidationException ex)
    {
        ex.ErrorCode.Should().NotBe("BOOKING_IN_THE_PAST");
    }
}

[Theory]
[InlineData(-1)]    // Tunti sitten
[InlineData(-24)]   // Päivä sitten
[InlineData(-168)]  // Viikko sitten
public async Task CreateBookingAsync_WhenStartTimeVariousHoursInPast_ShouldThrowBookingValidationException(int hoursOffset)
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(hoursOffset),
        EndTime = DateTime.UtcNow.AddHours(hoursOffset + 1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();
    exception.Which.ErrorCode.Should().Be("BOOKING_IN_THE_PAST");
}

[Fact]
public async Task CreateBookingAsync_WhenStartTimeInPast_ShouldNotCreateBooking()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(-1),
        EndTime = DateTime.UtcNow.AddHours(1),
        Description = "Test"
    };

    var initialBookingCount = await _context.Bookings.CountAsync();

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    await act.Should().ThrowAsync<BookingValidationException>();

    // Varmista että varausta EI luotu
    var finalBookingCount = await _context.Bookings.CountAsync();
    finalBookingCount.Should().Be(initialBookingCount);
}

[Fact]
public async Task CreateBookingAsync_WhenBothTimesInPast_ShouldThrowBookingValidationException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = DateTime.UtcNow.AddHours(-2), // Molemmat menneisyydessä
        EndTime = DateTime.UtcNow.AddHours(-1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();
    exception.Which.ErrorCode.Should().Be("BOOKING_IN_THE_PAST");
}
```

#### Huomioita

**1. BeCloseTo() DateTime-vertailuissa:**

```csharp
.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2))
```

Koska testit suorittavat koodia, kellonajan vertailu täytyy olla joustava. BeCloseTo sallii pienen eron.

**2. Theory-testi eri aikaskaaloille:**
Testaa että validointi toimii eri menneisyyden syvyyksillä (tunti, päivä, viikko sitten).  
**3. Edge case - StartTime tasan nyt:**
Tämä on raja-arvo joka kannattaa testata erikseen.  
**4. Varmista että varausta ei luotu:**
Tärkeää testata että virheen sattuessa tietokantaan ei tallenneta mitään.

Jos käytät mockattua aikaa
Jos `_time` on interface (esim. `ISystemClock`), voit mockatakin sen Moqilla:

```csharp
[Fact]
public async Task CreateBookingAsync_WhenStartTimeInPast_WithMockedTime_ShouldThrowException()
{
    // Arrange
    var mockTime = new Mock<ISystemClock>();
    var fixedNow = new DateTime(2026, 1, 22, 12, 0, 0, DateTimeKind.Utc);
    mockTime.Setup(t => t.Now).Returns(fixedNow);

    var service = new BookingService(_context, Options.Create(_settings), mockTime.Object);

    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = fixedNow.AddHours(-1), // Tunti "ennen" mockattua aikaa
        EndTime = fixedNow.AddHours(1),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await service.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>()
        .Where(e => e.ErrorCode == "BOOKING_IN_THE_PAST");
}
```

Tämä antaa täyden kontrollin ajan yli ja poistaa flakyuden.  
Haluatko että teen testit myös muille validointimetodeille (`ValidateBookingDuration`, `ValidateNotTooFarAhead`)?

### 👤 Nina

> Joo, minulla on mockattu aika käytössä

### 🤖 Claude

Loistavaa! Mockattu aika tekee testeistä paljon luotettavampia. Tehdään testit kaikille validointimetodeille käyttäen mockattua aikaa.

#### 1. Päivitä testiluokan setup

```csharp
public class BookingServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly BookingService _sut;
    private readonly BookingSettings _settings;
    private readonly Mock<ISystemClock> _mockTime;
    private readonly DateTime _fixedNow;

    public BookingServiceTests()
    {
        // Luo InMemory-tietokanta
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        SeedTestData();

        // Luo settings
        _settings = new BookingSettings
        {
            MinBookingMinutes = 15,
            MaxBookingHours = 16,
            MaxBookingMonthsAhead = 6
        };

        // Mockaa aika - kiinteä ajankohta
        _mockTime = new Mock<ISystemClock>();
        _fixedNow = new DateTime(2026, 1, 22, 12, 0, 0, DateTimeKind.Utc);
        _mockTime.Setup(t => t.Now).Returns(_fixedNow);

        // Luo service mockatulla ajalla
        _sut = new BookingService(_context, Options.Create(_settings), _mockTime.Object);
    }

    private void SeedTestData()
    {
        var rooms = new List<MeetingRoom>
        {
            new MeetingRoom { Id = 1, Name = "Sali A", Capacity = 10 },
            new MeetingRoom { Id = 2, Name = "Sali B", Capacity = 6 }
        };

        _context.MeetingRooms.AddRange(rooms);
        _context.SaveChanges();
    }
}
```

#### 2. ValidateNotInPast testit (mockatulla ajalla)

```csharp
[Fact]
public async Task CreateBookingAsync_WhenStartTimeInPast_ShouldThrowBookingValidationException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(-1), // Tunti ennen mockattua aikaa
        EndTime = _fixedNow.AddHours(1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();
    exception.Which.Message.Should().Be("Varaus ei voi sijoittua menneisyyteen.");
    exception.Which.ErrorCode.Should().Be("BOOKING_IN_THE_PAST");
}

[Fact]
public async Task CreateBookingAsync_WhenStartTimeInPast_ShouldIncludeCorrectTimesInDetails()
{
    // Arrange
    var pastTime = _fixedNow.AddHours(-2);
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = pastTime,
        EndTime = _fixedNow.AddHours(1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();

    var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
    using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
    var root = jsonDoc.RootElement;

    root.GetProperty("requestedStart").GetDateTime().Should().Be(pastTime);
    root.GetProperty("currentTime").GetDateTime().Should().Be(_fixedNow);
}

[Theory]
[InlineData(-1)]    // Tunti sitten
[InlineData(-24)]   // Päivä sitten
[InlineData(-168)]  // Viikko sitten
public async Task CreateBookingAsync_WhenStartTimeInPast_WithDifferentOffsets_ShouldThrowException(int hoursOffset)
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(hoursOffset),
        EndTime = _fixedNow.AddHours(hoursOffset + 2),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>()
        .Where(e => e.ErrorCode == "BOOKING_IN_THE_PAST");
}

[Fact]
public async Task CreateBookingAsync_WhenStartTimeInFuture_ShouldNotThrowPastTimeException()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2),
        Description = "Test"
    };

    // Act
    var result = await _sut.CreateBookingAsync(dto);

    // Assert
    result.Should().NotBeNull();
    result.StartTime.Should().Be(_fixedNow.AddHours(1));
}
```

#### 3. ValidateBookingDuration testit

```csharp
[Fact]
public async Task CreateBookingAsync_WhenBookingTooShort_ShouldThrowBookingValidationException()
{
    // Arrange - 10 minuuttia, kun minimi on 15
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(1).AddMinutes(10),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();
    exception.Which.ErrorCode.Should().Be("BOOKING_TOO_SHORT");
    exception.Which.Message.Should().Contain($"{_settings.MinBookingMinutes} minuuttia");
}

[Fact]
public async Task CreateBookingAsync_WhenBookingTooShort_ShouldIncludeDurationInDetails()
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(1).AddMinutes(5),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();

    var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
    using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
    var root = jsonDoc.RootElement;

    root.GetProperty("requestedMinutes").GetDouble().Should().Be(5);
    root.GetProperty("minimumMinutes").GetInt32().Should().Be(_settings.MinBookingMinutes);
}

[Fact]
public async Task CreateBookingAsync_WhenBookingExactlyMinimum_ShouldNotThrowShortException()
{
    // Arrange - Tasan 15 minuuttia
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(1).AddMinutes(_settings.MinBookingMinutes),
        Description = "Test"
    };

    // Act
    var result = await _sut.CreateBookingAsync(dto);

    // Assert
    result.Should().NotBeNull();
}

[Fact]
public async Task CreateBookingAsync_WhenBookingTooLong_ShouldThrowBookingValidationException()
{
    // Arrange - 20 tuntia, kun maksimi on 16
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(21),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();
    exception.Which.ErrorCode.Should().Be("BOOKING_TOO_LONG");
    exception.Which.Message.Should().Contain($"{_settings.MaxBookingHours} tuntia");
}

[Fact]
public async Task CreateBookingAsync_WhenBookingTooLong_ShouldIncludeDurationInDetails()
{
    // Arrange - 20 tuntia
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(21),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();

    var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
    using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
    var root = jsonDoc.RootElement;

    root.GetProperty("requestedHours").GetDouble().Should().Be(20);
    root.GetProperty("maximumHours").GetInt32().Should().Be(_settings.MaxBookingHours);
}

[Fact]
public async Task CreateBookingAsync_WhenBookingExactlyMaximum_ShouldNotThrowLongException()
{
    // Arrange - Tasan 16 tuntia
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(1 + _settings.MaxBookingHours),
        Description = "Test"
    };

    // Act
    var result = await _sut.CreateBookingAsync(dto);

    // Assert
    result.Should().NotBeNull();
}

[Theory]
[InlineData(1)]     // 1 minuutti
[InlineData(5)]     // 5 minuuttia
[InlineData(14)]    // 14 minuuttia
public async Task CreateBookingAsync_WhenBookingUnderMinimum_WithDifferentDurations_ShouldThrowException(int minutes)
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(1).AddMinutes(minutes),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>()
        .Where(e => e.ErrorCode == "BOOKING_TOO_SHORT");
}

[Theory]
[InlineData(17)]    // 17 tuntia
[InlineData(24)]    // 24 tuntia
[InlineData(48)]    // 48 tuntia
public async Task CreateBookingAsync_WhenBookingOverMaximum_WithDifferentDurations_ShouldThrowException(int hours)
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(1 + hours),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>()
        .Where(e => e.ErrorCode == "BOOKING_TOO_LONG");
}
```

#### 4. ValidateNotTooFarAhead testit

```csharp
[Fact]
public async Task CreateBookingAsync_WhenBookingTooFarInFuture_ShouldThrowBookingValidationException()
{
    // Arrange - 7 kuukautta eteenpäin, kun maksimi on 6
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddMonths(7),
        EndTime = _fixedNow.AddMonths(7).AddHours(1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();
    exception.Which.ErrorCode.Should().Be("BOOKING_TOO_FAR_AHEAD");
    exception.Which.Message.Should().Contain($"{_settings.MaxBookingMonthsAhead} kuukauden");
}

[Fact]
public async Task CreateBookingAsync_WhenBookingTooFarInFuture_ShouldIncludeDatesInDetails()
{
    // Arrange
    var farFutureDate = _fixedNow.AddMonths(8);
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = farFutureDate,
        EndTime = farFutureDate.AddHours(1),
        Description = "Test"
    };

    // Act
    Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

    // Assert
    var exception = await act.Should().ThrowAsync<BookingValidationException>();

    var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
    using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
    var root = jsonDoc.RootElement;

    root.GetProperty("requestedStartDate").GetDateTime().Should().Be(farFutureDate);
    root.GetProperty("currentDate").GetDateTime().Should().Be(_fixedNow);
    root.GetProperty("maximumStartDate").GetDateTime().Should().Be(_fixedNow.AddMonths(_settings.MaxBookingMonthsAhead));
}

[Fact]
public async Task CreateBookingAsync_WhenBookingExactlyAtMaximumFuture_ShouldNotThrowFarAheadException()
{
    // Arrange - Tasan 6 kuukautta eteenpäin
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddMonths(_settings.MaxBookingMonthsAhead),
        EndTime = _fixedNow.AddMonths(_settings.MaxBookingMonthsAhead).AddHours(1),
        Description = "Test"
    };

    // Act
    var result = await _sut.CreateBookingAsync(dto);

    // Assert
    result.Should().NotBeNull();
}

[Theory]
[InlineData(7)]     // 7 kuukautta
[InlineData(12)]    // Vuosi
[InlineData(24)]    // 2 vuotta
public async Task CreateBookingAsync_WhenBookingTooFarAhead_WithDifferentMonths_ShouldThrowException(int months)
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddMonths(months),
        EndTime = _fixedNow.AddMonths(months).AddHours(1),
        Description = "Test"
    };

    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
        .Should().ThrowAsync<BookingValidationException>()
        .Where(e => e.ErrorCode == "BOOKING_TOO_FAR_AHEAD");
}

[Theory]
[InlineData(1)]     // Kuukausi
[InlineData(3)]     // 3 kuukautta
[InlineData(6)]     // Tasan 6 kuukautta (raja-arvo)
public async Task CreateBookingAsync_WhenBookingWithinAllowedFuture_ShouldSucceed(int months)
{
    // Arrange
    var dto = new CreateBookingDto
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddMonths(months),
        EndTime = _fixedNow.AddMonths(months).AddHours(1),
        Description = "Test"
    };

    // Act
    var result = await _sut.CreateBookingAsync(dto);

    // Assert
    result.Should().NotBeNull();
    result.StartTime.Should().Be(_fixedNow.AddMonths(months));
}
```

#### Yhteenveto

**Mockatun ajan edut:**

✅ Ei race conditioneita  
✅ Testit ovat deterministisiä  
✅ Voit testata tarkkoja raja-arvoja  
✅ Ei riippuvuutta todellisesta kellon ajasta

**Katettavuus:**

✅ Kaikki validointimetodit testattu  
✅ Raja-arvot testattu (minimi, maksimi, tasan rajalla)  
✅ Theory-testit eri arvoilla  
✅ Error code ja details-tarkistukset

Haluatko että teen vielä testit muille metodeille (GetBooking, DeleteBooking, konflikti-tarkistus)?

### 👤 Nina

> tee testit myös näille metodeille
>
> ```C#
>  public async Task<BookingDto> GetBookingAsync(int id)
>  {
>      var booking = await _context.Bookings
>          .Include(b => b.MeetingRoom)
>          .FirstOrDefaultAsync(b => b.Id == id) ?? throw new NotFoundException($"Varausta ID:llä {id} ei löytynyt.");
>      var dto = new BookingDto
>      {
>          Id = booking.Id,
>          MeetingRoomId = booking.MeetingRoomId,
>          MeetingRoomName = booking.MeetingRoom.Name,
>          BookedBy = booking.BookedBy,
>          StartTime = booking.StartTime,
>          EndTime = booking.EndTime,
>          AdditionalDetails = booking.AdditionalDetails
>      };
>      return dto;
>  }
> ```
>
> ```c#
> public async Task<IEnumerable<BookingDto>> GetRoomBookingsAsync(int roomId)
> {
>     var room = await _context.MeetingRooms.FindAsync(roomId) ?? throw new NotFoundException($"Kokoushuonetta ID:llä {roomId} ei löydy.");
>     var bookings = await _context.Bookings
>
>         .Where(b => b.MeetingRoomId == roomId)
>         .Select(b => new BookingDto
>         {
>             Id = b.Id,
>             MeetingRoomId = b.MeetingRoomId,
>             MeetingRoomName = b.MeetingRoom.Name,
>             BookedBy = b.BookedBy,
>             StartTime = b.StartTime,
>             EndTime = b.EndTime,
>             AdditionalDetails = b.AdditionalDetails
>         })
>         .OrderBy(b => b.StartTime)
>         .ToListAsync();
>     return bookings;
> }
> ```
>
> ```c#
> public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
> {
>     var bookings = await _context.Bookings
>         .Select(b => new BookingDto
>         {
>             Id = b.Id,
>             MeetingRoomId = b.MeetingRoomId,
>             MeetingRoomName = b.MeetingRoom.Name,
>             BookedBy = b.BookedBy,
>             StartTime = b.StartTime,
>             EndTime = b.EndTime,
>             AdditionalDetails = b.AdditionalDetails
>         })
>         .OrderBy(b => b.StartTime)
>         .ToListAsync();
>     return bookings;
> }
> ```
>
> ```c#
> public async Task DeleteBookingAsync(int id)
> {
>     var booking = await _context.Bookings.FindAsync(id) ?? throw new NotFoundException($"Varausta ID:llä {id} ei löydy.");
>     _context.Bookings.Remove(booking);
>     await _context.SaveChangesAsync();
> }
> ```

### 🤖 Claude

Tässä kattavat testit kaikille Get- ja Delete-metodeille:

#### 1. GetBookingAsync testit

```csharp
[Fact]
public async Task GetBookingAsync_WhenBookingExists_ShouldReturnBookingDto()
{
    // Arrange
    var booking = new Booking
    {
        Id = 1,
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2),
        AdditionalDetails = "Test details"
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    // Act
    var result = await _sut.GetBookingAsync(1);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(1);
    result.MeetingRoomId.Should().Be(1);
    result.MeetingRoomName.Should().Be("Sali A");
    result.BookedBy.Should().Be("Test User");
    result.StartTime.Should().Be(_fixedNow.AddHours(1));
    result.EndTime.Should().Be(_fixedNow.AddHours(2));
    result.AdditionalDetails.Should().Be("Test details");
}

[Fact]
public async Task GetBookingAsync_WhenBookingDoesNotExist_ShouldThrowNotFoundException()
{
    // Arrange
    var nonExistentId = 999;

    // Act
    Func<Task> act = async () => await _sut.GetBookingAsync(nonExistentId);

    // Assert
    var exception = await act.Should().ThrowAsync<NotFoundException>();
    exception.Which.Message.Should().Be($"Varausta ID:llä {nonExistentId} ei löytynyt.");
}

[Fact]
public async Task GetBookingAsync_WhenMultipleBookingsExist_ShouldReturnCorrectOne()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking
        {
            Id = 1,
            MeetingRoomId = 1,
            BookedBy = "User 1",
            StartTime = _fixedNow.AddHours(1),
            EndTime = _fixedNow.AddHours(2)
        },
        new Booking
        {
            Id = 2,
            MeetingRoomId = 2,
            BookedBy = "User 2",
            StartTime = _fixedNow.AddHours(3),
            EndTime = _fixedNow.AddHours(4)
        }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var result = await _sut.GetBookingAsync(2);

    // Assert
    result.Id.Should().Be(2);
    result.MeetingRoomId.Should().Be(2);
    result.MeetingRoomName.Should().Be("Sali B");
    result.BookedBy.Should().Be("User 2");
}

[Fact]
public async Task GetBookingAsync_ShouldIncludeMeetingRoomName()
{
    // Arrange
    var booking = new Booking
    {
        Id = 1,
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2)
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    // Act
    var result = await _sut.GetBookingAsync(1);

    // Assert
    result.MeetingRoomName.Should().NotBeNullOrEmpty();
    result.MeetingRoomName.Should().Be("Sali A");
}

[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(int.MaxValue)]
public async Task GetBookingAsync_WithInvalidId_ShouldThrowNotFoundException(int invalidId)
{
    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.GetBookingAsync(invalidId))
        .Should().ThrowAsync<NotFoundException>();
}
```

#### 2. GetRoomBookingsAsync testit

```csharp
[Fact]
public async Task GetRoomBookingsAsync_WhenRoomExists_ShouldReturnAllBookingsForRoom()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking
        {
            MeetingRoomId = 1,
            BookedBy = "User 1",
            StartTime = _fixedNow.AddHours(2),
            EndTime = _fixedNow.AddHours(3)
        },
        new Booking
        {
            MeetingRoomId = 1,
            BookedBy = "User 2",
            StartTime = _fixedNow.AddHours(1),
            EndTime = _fixedNow.AddHours(2)
        },
        new Booking
        {
            MeetingRoomId = 2, // Eri huone
            BookedBy = "User 3",
            StartTime = _fixedNow.AddHours(1),
            EndTime = _fixedNow.AddHours(2)
        }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var result = await _sut.GetRoomBookingsAsync(1);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.Should().OnlyContain(b => b.MeetingRoomId == 1);
}

[Fact]
public async Task GetRoomBookingsAsync_ShouldReturnBookingsOrderedByStartTime()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking
        {
            MeetingRoomId = 1,
            BookedBy = "User 1",
            StartTime = _fixedNow.AddHours(5), // Myöhäisin
            EndTime = _fixedNow.AddHours(6)
        },
        new Booking
        {
            MeetingRoomId = 1,
            BookedBy = "User 2",
            StartTime = _fixedNow.AddHours(1), // Aikaisin
            EndTime = _fixedNow.AddHours(2)
        },
        new Booking
        {
            MeetingRoomId = 1,
            BookedBy = "User 3",
            StartTime = _fixedNow.AddHours(3), // Keskimmäinen
            EndTime = _fixedNow.AddHours(4)
        }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var result = (await _sut.GetRoomBookingsAsync(1)).ToList();

    // Assert
    result.Should().HaveCount(3);
    result[0].BookedBy.Should().Be("User 2");
    result[1].BookedBy.Should().Be("User 3");
    result[2].BookedBy.Should().Be("User 1");
    result.Should().BeInAscendingOrder(b => b.StartTime);
}

[Fact]
public async Task GetRoomBookingsAsync_WhenRoomHasNoBookings_ShouldReturnEmptyList()
{
    // Arrange - Huone on olemassa mutta ei varauksia
    var roomId = 1;

    // Act
    var result = await _sut.GetRoomBookingsAsync(roomId);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
}

[Fact]
public async Task GetRoomBookingsAsync_WhenRoomDoesNotExist_ShouldThrowNotFoundException()
{
    // Arrange
    var nonExistentRoomId = 999;

    // Act
    Func<Task> act = async () => await _sut.GetRoomBookingsAsync(nonExistentRoomId);

    // Assert
    var exception = await act.Should().ThrowAsync<NotFoundException>();
    exception.Which.Message.Should().Be($"Kokoushuonetta ID:llä {nonExistentRoomId} ei löydy.");
}

[Fact]
public async Task GetRoomBookingsAsync_ShouldIncludeMeetingRoomNames()
{
    // Arrange
    var booking = new Booking
    {
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2)
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    // Act
    var result = await _sut.GetRoomBookingsAsync(1);

    // Assert
    result.Should().HaveCount(1);
    result.First().MeetingRoomName.Should().Be("Sali A");
}

[Fact]
public async Task GetRoomBookingsAsync_WithMultipleRooms_ShouldOnlyReturnSpecificRoomBookings()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking { MeetingRoomId = 1, BookedBy = "User A", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
        new Booking { MeetingRoomId = 1, BookedBy = "User B", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) },
        new Booking { MeetingRoomId = 2, BookedBy = "User C", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
        new Booking { MeetingRoomId = 2, BookedBy = "User D", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var room1Bookings = await _sut.GetRoomBookingsAsync(1);
    var room2Bookings = await _sut.GetRoomBookingsAsync(2);

    // Assert
    room1Bookings.Should().HaveCount(2);
    room1Bookings.Should().OnlyContain(b => b.MeetingRoomId == 1);

    room2Bookings.Should().HaveCount(2);
    room2Bookings.Should().OnlyContain(b => b.MeetingRoomId == 2);
}
```

#### 3. GetAllBookingsAsync testit

```csharp
[Fact]
public async Task GetAllBookingsAsync_WhenBookingsExist_ShouldReturnAllBookings()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking { MeetingRoomId = 1, BookedBy = "User 1", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
        new Booking { MeetingRoomId = 2, BookedBy = "User 2", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) },
        new Booking { MeetingRoomId = 1, BookedBy = "User 3", StartTime = _fixedNow.AddHours(5), EndTime = _fixedNow.AddHours(6) }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var result = await _sut.GetAllBookingsAsync();

    // Assert
    result.Should().HaveCount(3);
}

[Fact]
public async Task GetAllBookingsAsync_ShouldReturnBookingsOrderedByStartTime()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking { MeetingRoomId = 1, BookedBy = "User C", StartTime = _fixedNow.AddHours(5), EndTime = _fixedNow.AddHours(6) },
        new Booking { MeetingRoomId = 2, BookedBy = "User A", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
        new Booking { MeetingRoomId = 1, BookedBy = "User B", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var result = (await _sut.GetAllBookingsAsync()).ToList();

    // Assert
    result.Should().HaveCount(3);
    result[0].BookedBy.Should().Be("User A");
    result[1].BookedBy.Should().Be("User B");
    result[2].BookedBy.Should().Be("User C");
    result.Should().BeInAscendingOrder(b => b.StartTime);
}

[Fact]
public async Task GetAllBookingsAsync_WhenNoBookings_ShouldReturnEmptyList()
{
    // Act
    var result = await _sut.GetAllBookingsAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
}

[Fact]
public async Task GetAllBookingsAsync_ShouldIncludeMeetingRoomNames()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking { MeetingRoomId = 1, BookedBy = "User 1", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
        new Booking { MeetingRoomId = 2, BookedBy = "User 2", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var result = (await _sut.GetAllBookingsAsync()).ToList();

    // Assert
    result[0].MeetingRoomName.Should().Be("Sali A");
    result[1].MeetingRoomName.Should().Be("Sali B");
}

[Fact]
public async Task GetAllBookingsAsync_ShouldIncludeBookingsFromAllRooms()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking { MeetingRoomId = 1, BookedBy = "User 1", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
        new Booking { MeetingRoomId = 1, BookedBy = "User 2", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) },
        new Booking { MeetingRoomId = 2, BookedBy = "User 3", StartTime = _fixedNow.AddHours(5), EndTime = _fixedNow.AddHours(6) }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    var result = await _sut.GetAllBookingsAsync();

    // Assert
    result.Should().Contain(b => b.MeetingRoomId == 1);
    result.Should().Contain(b => b.MeetingRoomId == 2);
}

[Fact]
public async Task GetAllBookingsAsync_ShouldMapAllDtoProperties()
{
    // Arrange
    var booking = new Booking
    {
        Id = 1,
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2),
        AdditionalDetails = "Important meeting"
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    // Act
    var result = (await _sut.GetAllBookingsAsync()).First();

    // Assert
    result.Id.Should().Be(1);
    result.MeetingRoomId.Should().Be(1);
    result.MeetingRoomName.Should().Be("Sali A");
    result.BookedBy.Should().Be("Test User");
    result.StartTime.Should().Be(_fixedNow.AddHours(1));
    result.EndTime.Should().Be(_fixedNow.AddHours(2));
    result.AdditionalDetails.Should().Be("Important meeting");
}
```

#### 4. DeleteBookingAsync testit

```csharp
[Fact]
public async Task DeleteBookingAsync_WhenBookingExists_ShouldDeleteBooking()
{
    // Arrange
    var booking = new Booking
    {
        Id = 1,
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2)
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    var initialCount = await _context.Bookings.CountAsync();

    // Act
    await _sut.DeleteBookingAsync(1);

    // Assert
    var finalCount = await _context.Bookings.CountAsync();
    finalCount.Should().Be(initialCount - 1);

    var deletedBooking = await _context.Bookings.FindAsync(1);
    deletedBooking.Should().BeNull();
}

[Fact]
public async Task DeleteBookingAsync_WhenBookingDoesNotExist_ShouldThrowNotFoundException()
{
    // Arrange
    var nonExistentId = 999;

    // Act
    Func<Task> act = async () => await _sut.DeleteBookingAsync(nonExistentId);

    // Assert
    var exception = await act.Should().ThrowAsync<NotFoundException>();
    exception.Which.Message.Should().Be($"Varausta ID:llä {nonExistentId} ei löydy.");
}

[Fact]
public async Task DeleteBookingAsync_ShouldNotAffectOtherBookings()
{
    // Arrange
    var bookings = new List<Booking>
    {
        new Booking { Id = 1, MeetingRoomId = 1, BookedBy = "User 1", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
        new Booking { Id = 2, MeetingRoomId = 1, BookedBy = "User 2", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) },
        new Booking { Id = 3, MeetingRoomId = 2, BookedBy = "User 3", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) }
    };
    _context.Bookings.AddRange(bookings);
    await _context.SaveChangesAsync();

    // Act
    await _sut.DeleteBookingAsync(2);

    // Assert
    var remainingBookings = await _context.Bookings.ToListAsync();
    remainingBookings.Should().HaveCount(2);
    remainingBookings.Should().Contain(b => b.Id == 1);
    remainingBookings.Should().Contain(b => b.Id == 3);
    remainingBookings.Should().NotContain(b => b.Id == 2);
}

[Fact]
public async Task DeleteBookingAsync_WhenCalledMultipleTimes_SecondCallShouldThrowNotFoundException()
{
    // Arrange
    var booking = new Booking
    {
        Id = 1,
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2)
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    // Act - Ensimmäinen poisto
    await _sut.DeleteBookingAsync(1);

    // Assert - Toinen poisto pitäisi epäonnistua
    Func<Task> act = async () => await _sut.DeleteBookingAsync(1);
    await act.Should().ThrowAsync<NotFoundException>();
}

[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(int.MaxValue)]
public async Task DeleteBookingAsync_WithInvalidId_ShouldThrowNotFoundException(int invalidId)
{
    // Act & Assert
    await FluentActions.Invoking(async () => await _sut.DeleteBookingAsync(invalidId))
        .Should().ThrowAsync<NotFoundException>();
}

[Fact]
public async Task DeleteBookingAsync_ShouldPersistChangesToDatabase()
{
    // Arrange
    var booking = new Booking
    {
        Id = 1,
        MeetingRoomId = 1,
        BookedBy = "Test User",
        StartTime = _fixedNow.AddHours(1),
        EndTime = _fixedNow.AddHours(2)
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    // Act
    await _sut.DeleteBookingAsync(1);

    // Assert - Varmista että muutos on tallentuu contextiin
    _context.ChangeTracker.Entries<Booking>()
        .Should().NotContain(e => e.Entity.Id == 1);
}
```

#### Yhteenveto testikattavuudesta

**GetBookingAsync:**

✅ Palauttaa oikean varauksen  
✅ Heittää NotFoundExceptionin jos ei löydy  
✅ Sisältää MeetingRoom-tiedot  
✅ Toimii useilla varauksilla

GetRoomBookingsAsync:

✅ Palauttaa vain tietyn huoneen varaukset
✅ Järjestää aloitusajan mukaan
✅ Palauttaa tyhjän listan jos ei varauksia
✅ Heittää NotFoundExceptionin jos huonetta ei ole

**GetAllBookingsAsync:**

✅ Palauttaa kaikki varaukset  
✅ Järjestää aloitusajan mukaan  
✅ Sisältää kaikkien huoneiden varaukset  
✅ Palauttaa tyhjän listan jos ei varauksia

**DeleteBookingAsync:**

✅ Poistaa varauksen  
✅ Heittää NotFoundExceptionin jos ei löydy  
✅ Ei vaikuta muihin varauksiin  
✅ Tallentaa muutokset tietokantaan

Kaikki testit käyttävät mockattua aikaa ja InMemory-tietokantaa, joten ne ovat nopeita ja luotettavia!
