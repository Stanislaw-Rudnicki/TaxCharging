# Komponent liczący podatek od wynagrodzenia pracownika

#### Zadanie:
Napisać komponent (/-y) liczące podatek od wynagrodzenia pracownika.<br>
O ile to możliwe, należy utworzyć solucję w Visual Studio. Kod liczący należy umieścic w projekcie typu "C# Class Library".<br>
Należy oczywiście zadbać o to, by obliczenia działały poprawnie, a kod był napisany "zgodnie ze sztuką".

Algorytm jest wymyślony na potrzeby tego zadania. Trochę przypomina on mocno uproszczony polski system podatkowy.

#### Opis:
Od każdego wynagrodzenia powinien zostać naliczony podatek dochodowy.<br>
W zależności od sposobu rozliczania się danego pracownika z fiskusem (jeden konkretny sposób, niezmienny w roku kalendarzowym), może to być:
- zwolnienie z podatku
- podatek liniowy (sztywne 19&nbsp;% procent od wynagrodzenia)
- podatek progresywny:
	- pierwsze 85528 złotych wynagrodzeń pracownika w roku kalendarzowym jest obłożone podatkiem 17&nbsp;%
	- wynagrodzenia, które narastąjąco w roku przekraczają 85528 złotych obłożone są podatkiem 32&nbsp;%
	- pierwsze 525,12&nbsp;zł naliczonego podatku w roku kalendarzowym nie musi być płacone (pomniejszamy podatek)
Tak wyliczony podatek zaokrągla się do pełnych złotych w dół.

#### Scenariusz dodatkowy:
Wynagrodzenie może być wypłacone w walucie obcej np 500 euro. Podatek zawsze nalicza się w złotych, więc do wynagrodzenia najpierw należy zastosować przelicznik na złotówki po kursie danej waluty z dnia wypłaty. Przeliczniki nie muszą być rzeczywiste i mogą być zawarte w kodzie źródłowym.<br>
Rozszerzenie scenariusza dodatkowego: Jedno wynagrodzenie może być wypłacone wielu walutach naraz, np 200 złotych + 300 euro + 250 dolarów. Jak powyżej, także należy przeliczyć wynagrodzenie na złotówki.

#### Przykład:
Pracownik rozlicza się w danym roku podatkiem progresywnym. Pierwsze wynagrodzenie w tym roku w wysokości 5000 złotych. Mieści się w w całości limicie 85528, więc stosuje się stawkę 17&nbsp;%, co daje 850 złotych. Dodatkowo pomniejszone o 525,12 złotych daje 324,88. Zaokrąglone w dól daje finalnie 324 złote.<br>
Kolejne wynagrodzenie w tym roku w wysokości 100000 złotych. Uwzględniając poprzednie wynagrodzenie (5000&nbsp;zł) wychodzi 80528&nbsp;zł mieszczących się w limicie 85528 (stawka 17&nbsp;%) i 19472&nbsp;zł ponad ten limit (stawka 32&nbsp;%). Daje to 13689,76 + 6231,04 = 19920,80. Pomniejszenie podatku o 525,12 zostało już w danym roku zastosowane, więc zostaje tylko zaokrąglić finalną wartość, co daje 19920 złotych. Aby uzyskać łączną kwotę podatku należy do kwoty 19920 dodać 324, co daje 20245&nbsp;zł.
