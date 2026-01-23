# ANALYYSI

## 1. Mitä tekoäly teki hyvin?

Tekoäly generoi ensimmäisen version API:sta aika tarkalleen niinkuin pyysin, ja vähän enemmänkin. Ensimmäisessä kehotteessani kerroin mitä tekniikoita haluan käyttää sekä kopioin tehtävänannossa annetut speksit API:lle. Tekoäly teki InMemory- tietokannan ja osasi injektoida sen oikein sovellukseen. Tekoälyn generoimat MeetingRoom ja Booking -entityt olivat mielestäni järkeviä, eikä niitä tarvinut juuri muuttaa. Rajapinnat toimivat, ja pyytämieni POST / DELETE / GET -rajapintojen lisäksi tekoäly oli generoinut kaksi muuta rajapintaa: Toisella voi hakea kaikki varaukset kaikista huoneista ja toisella voi hakea varausta varauksen ID:n perusteella. Mielestäni ne olivat ihan hyödyllisiä, joten jätin ne sovellukseen.

Tekoäly osasi myös ohjata hyvin esim. kansiorakenteen luomisessa, ja kertoi mikä tiedosto kuuluu mihinkin kansioon. Kansiorakennetta tai kansioiden nimiäkään minun ei tarvinut muuttaa.

Testaillessani APIa en löytänyt bugeja, ja virheviestit olivat myös järkevän oloisia. Virheiden statuskoodit oli oikein.

Myöhemmin käytin tekoälyä paljon koodin selittämiseen, ja se selitti logiikkaa ja työsttevää asiaa paljon myös pyytämättä jokaisen kehotteen yhdeydessä. Pyysin tekoälyä perustelemaan tekemiään ratkaisuja, ja sain järkeviä vastauksia. Esimerkiksi, kun pyysin tekoälyä selittämään BadRequest ja Conflict -tyyppien eroja, niin se selitti asian hyvin ymmärrettävästi ja selkein esimerkein.

(Suhtaudun tekoälyn antamiin vastauksiin/tutorointiin aina kuitenkin pienellä kriittisyydellä, ja mikäli minulla ei ole asiasta muuta tietoa, varmistan tekoälyn antamatat vastaukset dokumentaatiosta tai muista luotettavista lähteistä.)

## 2. Mitä tekoäly teki huonosti?

Tekoälyn generoimassa koodissa oli jonkin verran pieniä virheitä, joita en ihan heti ensisilmäyksellä huomannut. Esimerkiksi DTO:n nimeämisessä ei oltu käytetty Dto-loppua (esim. ErrorResponseDto oli tekoälyn versiossa vain ErrorResponse), Post- kontrollerista puuttui [FromBody] -attribuutti, ja yhteen Exceptioniin ei oltu asetettu kuvaavaa virheviestiä, kun kaikkiin muihin oli. Aika nopeasti opin, että tekoälyn tuottamaa koodia pitää katselmoida varsin tarkasti, ja että kuinka tärkeää on se että itsellä on jonkinlainen käsitys siitä mitä koodissa pitäisi olla. Tekoäly tykkäsi myös laittaa yhteen tiedostoon paljon tavaraa, esim. kaikki DTO:t yhdessä tiedostossa, kaikki Exceptionit yhdessä tiedostossa.

Tekoäly teki myös varsin hankalasti luettavan esimerkin varausten päällekkäisyyksien tarkistamisen:

```c#
// Claude

var hasOverlap = await _context.Bookings
        .AnyAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
                ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

```

```c#
// korjattu

var conflict = await _context.Bookings
    .FirstOrDefaultAsync(b =>
        b.MeetingRoomId == dto.MeetingRoomId &&
        dto.StartTime < b.EndTime &&
        dto.EndTime > b.StartTime);
```

Käytin tekoälynä Clauden ilmaista versioita, jossa on rajoitus kuinka paljon kehotteita pystyy antamaan. Kun raja tulee täyteen, tekoälyn käyttö estetään muutamaksi tunniksi. Tämä hidasti hieman työskentelyä, ja tekoälyn ollessa lukossa toteutin ratkaisua itsenäisesti tai googlesta vastauksia etsimällä. Kun tein APIini muutoksia joista en ollut raportoinut teköälylle, niin sen oli tietenkin hankalampi pysyä kärryillä sovelluksestani ja minun piti soveltaa sen antamia esimerkkejä sopimaan omaan toteutukseeni. Mitä pidemmäksi tekoälyn kanssa käymäni keskustelu venyi, huomasin että se alkoi käydä hitaammaksi.

Kun lopuksi halusin vielä lisätä yksikkötestit sovellukseeni ja pyysin tekoälyä generoimaan testit, niin se tuotti paljon samankaltaisia testejä (esimerkiksi jonkin toiminnallisuuden testaaminen ensin Fact -testillä ja sitten Theory -testillä) ja liian spesifeitä testejä (esimerkiksi löytyykö vastauksesta jokin yksittäinen arvo, vaikka samaa asiaa oli testattu aikaisemmin dto-mappauksessa). Osa testien nimistä oli mielestäni myös huonoja. Luulen, että minulla meni testien korjaamiseen, päällekkäisyyksien poistamiseen ja tarkisteluun saman verran aikaa, kuin mitä olisi mennyt siihen että olisin tehnyt kaiken alusta alkaen itse.

## 3. Mitkä olivat tärkeimmät parannukset, jotka teit tekoälyn tuottamaan koodiin ja miksi?

### Toimintalogiikan siirtäminen Services- tasolle

- Controller vastaa vain HTTP-pyyntöjen (request) vastaanottamisesta ja HTTP-vastausten(response) palauttamisesta.
- Kaikki liiketoimintalogiikka eriytettynä omalle tasolleen --> Koodi pysyy selkeämpän'
- Helpottaa testausta

### Virheenkäsittely ErrorHandlingMiddlewareen

- Keskitetty virheenkäsittely selkeyttää koodia --> Services-tasolla vähemmän logiikkaa kun virheenkäsittely eriytetty
- Ei tarvetta try - catch lohkoille
- Yhtenäinen JSON kaikille virheille

### Lisärajoitukset toimintalogiikkaan

- Varauksen minimipituus 15 min --> Halutaan ehkäistä vahinkovarausten syntyminen
- Varauksen maksimipituus 24 h --> Halutaan estää älyttömien varausten tekeminen
- Varaus max 6 kk päähän nykyhetkestä --> Halutaan ehkäistä varmuuden vuoksi varailu. Oletuksena API on suunnattu työpaikan sisäiseen käyttöön, ja harvoin kukaan tietää yli puoli vuotta etukäteen tarvitsevansa neukkaria (jatkokehityksenä voisi tehdä admin-oikeudet, joilla rajoituksia voidaan kiertää, mutta ei relevanttia tässä vaiheessa)

### Nykyhetken käsittely SystemTime -Servicen kautta

- Helpottaa testattavuutta
- Aika ei ole globaali muuttuja, vaan riippuvuus joka voidaan injektoida

### Yksikkötestien lisääminen

- Omaan ohjelmistokehitystyöskentelyyni on iskostunut vahvasti oman koodin testaus, joten halusin tehdä testit myös tähän projektiin
- Voin varmistua siitä, että sovellus toimii haluamallani tavalla
- Jos haluan laajentaa sovellusta, huomaan helpommin mikäli jokin uusi toteutus rikkoo vanhaa
- Helppo testata rajatapaukset
