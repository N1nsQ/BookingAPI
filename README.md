# KOKOUSHUONEEN VARAUS API

ASP.NET Core Web API kokoushuoneiden varaamiseen ja hallintaan.

### ASP.NET Core Web API – valitut tekniikat

- C#
- Entity Framework Core
- InMemory Database
- xUnit
- Moq

### Toiminnallisuudet:

- Hae kaikki varaukset kaikista kokoushuoneista
- Hae varaukset tietystä kokoushuoneesta (Esim. Sali A)
- Hae tiettyä varausta varauksen ID:llä
- Tee uusi varaus
- Poista varaus

### Tarjolla on 3 kokoushuonetta:

- Sali A, ID = 1
- Sali B, ID = 2
- Sali C, ID = 3

Apissa ei tällä hetkellä ole autentikointia tai käyttöoikeusrajoituksia, joten kuka tahansa voi käyttää sitä.

### Rajoitukset

- Varaukset eivät voi sijoittua menneisyyteen
- Varauksen keston on oltava vähintään 15 minuuttia
- Varaus saa olla kestoltaan enintään 24 tuntia
- Aloitusajan on oltava ennen lopetusaikaa
- Et voi varata huonetta, jos samalle ajalle on jo olemassa varaus
- Varauksen voi tehdä alkamaan maksimissaan 6 kk päähän. Varauksenteko huomioidaan päivätasolla, eli varaushetken kellonaika ei vaikuta.

API käyttää InMemory -tietokantaa, eli tiedot ovat olemassa istunnon ajan. Kun suljet APIn, tietokanta tyhjenee.

### Rajapinnat

- **GET /api/Bookings**
  - Hae kaikki varaukset kaikista huoneista
  - Jos varauksia ei ole, palautetaan tyhjä lista `[]`
- **GET /api/Bookings/{id}**
  - Hae yhtä varausta id:llä
  - Jos id:llä ei löydy mitään, palautetaan 404 virheviesti
- **GET /api/Bookings/room/{roomId}**
  - Hae kaikki varaukset tietystä huoneesta
  - Jos huoneessa ei ole varauksia, palautetaan tyhjä lista `[]`
  - Jos annetulla id:lla ei löydy huonetta, annetaan 404 virheviesti
- **POST /api/Bookings**
  - Tee varaus kokoushuoneeseen
- **DELETE /api/Bookings/{id}**
  - Poista varaus id:n perusteella

### Virheenkäsittely

API palauttaa virhetilanteissa yhtenäisen virherakenteen:

- `Message` – käyttäjälle tarkoitettu virheviesti
- `StatusCode` – HTTP-statuskoodi
- `ErrorCode` – frontendille tarkoitettu virhekoodi (esim. `BOOKING_TOO_SHORT`)
- `TraceId` – pyyntöön liittyvä tunniste
- `Details` – lisätietoja, esim. päälleekäisen varauksen tiedot
- `Timestamp` – virheen ajankohta

**ErrorHandlingMiddleware**

- `BookingConflictException` – palautetaan, kun asiakas tekee validin pyynnön, mutta se ei onnistu sisäisen konfliktin takia
  - Esimerkki: Käyttäjä yrittää tehdä varausta, mutta huone on varattu
  - Kääntyy HTTP 409 Conflict -vastaukseksi
- `BookingValidationException` – palautetaan, kun asiakkaan lähettämä pyyntö ei vastaa validointisääntöjä
  - Esimerkki: Varauksen lopetusaika on ennen aloitusaikaa
  - Kääntyy HTTP 400 Bad Request -vastaukseksi.
- `NotFoundException` - palautetaan, kun pyydettyä resurssia ei löydy
  - Esimerkki: Kokoushuonetta ei löydy, ID:tä ei löydy
  - Kääntyy HTTP 404 Not Found -vastaukseksi.

### Esimerkki JSON

```json
{
  "meetingRoomId": 1,
  "bookedBy": "Maija Mehiläinen",
  "startTime": "2026-03-22T10:00:00",
  "endTime": "2026-03-22T11:00:00",
  "additionalDetails": "Sprint Review"
}
```
