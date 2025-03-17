# Tetris Játék - Felhasználói Dokumentáció


### Bevezetés
Ez egy klasszikus Tetris játék modern megvalósítása. A játék célja, hogy a lehulló különböző alakú blokkokat úgy forgassa és helyezze el a játékos, hogy teljes sorokat alakítson ki. Amikor egy sor teljesen feltöltődik blokkokkal, az eltűnik, és pontokat kap érte a játékos.

### Irányítás
- **Bal nyíl** - Blokk mozgatása balra
- **Jobb nyíl** - Blokk mozgatása jobbra
- **Le nyíl** - Blokk gyorsabb leesése
- **Fel nyíl** - Blokk forgatása az óramutató járásával megegyező irányba
- **Z billentyű** - Blokk forgatása az óramutató járásával ellentétes irányba
- **C billentyű** - Jelenlegi blokk megtartása/cseréje
- **Szóköz** - Blokk azonnali ledobása
- **ESC** - Játék szüneteltetése

### Játékmenet
1. **Pontszámítás**:
   - Sorok kitöltése: 100 × szint × kitöltött sorok száma
   - Blokk ledobása: 2 pont per cella
   - Blokk lehelyezése: 10 pont

2. **Szintek**:
   - A játékos minden 250 pont után szintet lép
   - Magasabb szinteken a blokkok gyorsabban esnek

3. **Különleges funkciók**:
   - **Blokk megtartása**: A "C" billentyűvel egy blokkot félretehet későbbi használatra
   - **Következő blokkok**: A játék jobb oldalán látható a következő 3 blokk
   - **Toplista**: A legjobb 10 pontszám mentésre kerül

4. **Játék vége**:
   - A játék akkor ér véget, amikor egy új blokk már nem fér el a pályán
   - A pontszám bekerülhet a toplistába, ha elég magas
