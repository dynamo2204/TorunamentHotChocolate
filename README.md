Po odpaleniu programu wchodzimy w Create Document

1. W query rejestrujemy graczy np.
   
mutation {
  p1: register(firstName: "Jan", lastName: "Kowalski", email: "jan@test.com", password: "123")
  p2: register(firstName: "Anna", lastName: "Nowak", email: "anna@test.com", password: "123")
}

2. Logujemy się, żeby pobrać token
   
mutation {
  login(email: "jan@test.com", password: "123")
}

Otrzymamy odpowiedź z loginem. Kopiujemy zawartość login. 
Wchodzimy w ustawienia po prawej do góry (ikona obok localhost) > Authorization > Type: Bearer > Token: wklejamy

3. Tworzymy turniej z naszą nazwą np.
   
mutation {
  createTournament(name: "Mistrzostwa IT") {
    id
    name
  }
}

Jeżeli chcemy więcej tournamentów to lepiej zapisać odpowiedź, żeby pamiętać ID każdego

4. Dodajemy uczestników

mutation {
  add1: addParticipant(tournamentId: 1, userId: 1)
  add2: addParticipant(tournamentId: 1, userId: 2)
}

5. Start turnieju

mutation {
  startTournament(tournamentId: 1)
}

6. Sprawdzamy nasz turniej
query {
  myMatches {
    id
    round
    player1 { firstName }
    player2 { firstName }
    winner { firstName }
  }
}

Jeżeli w odpowiedzi pokazuje, że nie masz uprawnień to jesteś niezalogowany


7. Rozegranie turnieju

mutation {
  playMatch(matchId: 1, winnerId: 1) {
    id
    winner { firstName }
  }
}

8. Pogląd drabinki turniejowej (nie trzeba być zalogowanym)

query {
  tournaments(where: { id: { eq: 1 } }) {
    name
    bracket {
      matches {
        id
        round
        player1 { firstName }
        player2 { firstName }
        winner { firstName }
      }
    }
  }
}

9 Uwagi końcowe
- System sam dostosowuje liczbę rund do liczby uczestników (np. 4 graczy = 2 rundy, 8 graczy = 3 rundy, itd.)
- Zwycięzca meczu jest automatycznie przenoszony do kolejnej rundy w drabince
- Dostęp do rozgrywania meczów i podglądu "moich gier" jest chroniony tokenem JWT
- Jeżeli chcemy zacząć od nowa wyłączamy program i usuwamy tournament.db, następnie uruchamiamy
