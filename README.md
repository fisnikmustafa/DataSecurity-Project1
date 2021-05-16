# DataSecurity-Project1
# Përmbajtja
1. Hyrje
2. Caeser Cipher
3. Console App
4. Desktop App

# Hyrje
Projekti është realizuar përmes Visual Studio 2019 IDE dhe gjuhës programuese C#.

Tema e këtij projekti është “Sulmimi i kodit të Cezarit duke mësuar frekuencat e shkronjave nga ndonjë tekst fajll”
dhe është punuar nga: Fisinik Mustafa dhe Gentrim Canolli.

# Caeser Cipher
Kodi i Cezarit është një nga kodet më të thjeshta dhe me të njohura. Ky kod bën një zëvendësim në të cilën çdo shkronjë në tekstin e thjeshtë 'zhvendoset' për një numër të caktuar. Për shembull, me një zhvendosje prej 1, A do të zëvendësohej nga B, B do të bëhej C, etj. Metoda është emërtuar pas Jul Cezarit, i cili e ka përdorur atë për të komunikuar me gjeneralët e tij. Enkriptimi behet duke i shndërruar shkronjat alfabeti latin në numra.
Për enkriptim përdoret formula : e(x) = (x + k) (mod 26), ndërsa për dekriptim: d(x) = (x - k)(mod 26). Ku 'k' paraqet çelësin(the shift).
Ndërsa për thyerjen e kodit të Cezarit, atë mund ta bëjmë përmes dy formave:
1. Brute Force Attack
2. Frequency Distribution of the Letters.

Ne do ta përdorim formën e dytë.

# Console App
Console App-i duket kështu , ku fillimisht përdoruesi zgjedh .txt file path dhe çelësin , e më pas shfaqen rezultatet:

<img width="660" alt="console" src="https://user-images.githubusercontent.com/74964656/118414385-822fe200-b6a4-11eb-9032-62cd5331bf51.png">

# Desktop App
 Pas zgjedhjes së .txt file , shtypjes së butonit READ e më pas Encrypt shfaqet:
 
<img width="380" alt="caeser1" src="https://user-images.githubusercontent.com/74964656/118414711-878e2c00-b6a6-11eb-9b55-bcd92a51bd5a.png">

Pas zgjedhjes së çelësit:

<img width="376" alt="caesar2" src="https://user-images.githubusercontent.com/74964656/118414768-c623e680-b6a6-11eb-9d97-e42ac43435f9.png">

Pas shtypjes së butoni Analyse and Crack:


<img width="379" alt="caesar3" src="https://user-images.githubusercontent.com/74964656/118414807-fbc8cf80-b6a6-11eb-8b43-4a68b0729639.png">

<img width="446" alt="caesar4" src="https://user-images.githubusercontent.com/74964656/118414829-1bf88e80-b6a7-11eb-9667-44243e1b53ab.png">





