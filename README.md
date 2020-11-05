# Sustav za preraspodjelu termina pregleda

## Zadatak

U jednom danu je schedulirano npr. 10 slijednih pregleda u trajanju od 30min od 12:00 do 17:00. Treći pregled se nenadano oduži za npr. 20 minuta, te postoji potreba za preraspodjelom tj. odgodom trajanja budućih pregleda u danu. Rješenje za ovaj zadatak je implementacija servisa koji će za N doktora raditi uživo preraspodjelu pregleda za tekući dan.

## Arhitektura sustava

<p align="center">
  <img src="https://user-images.githubusercontent.com/18721181/96591857-01fcf600-12e8-11eb-8719-daaa6558b595.png">
</p>

## Implementacija

* AppointmentAPI (.NET Core Web API): identifikacirane su putanje za pristupanje resursima te implementiranje metode nad resursima. Tako imamo 6 kreiranih metoda nad resursima: 
  - POST metoda (endpoint: /appointment) - dodavanje novog termina za pregled određenog pacijenta
  - PUT metoda (endpoint: /doctor/{doctorId}/appointment/{id}) - izmjena termina postojećeg pregleda određenog pacijenta
  - DELETE metoda (endpoint: /doctor/{doctorId}/appointment/{id}) - brisanje termina pregleda određenog pacijenta
  - DELETE metoda (endpoint: /doctor/{doctorId}) - brisanje svih termina za određeni dan za određenog doktora
  - GET metoda (endpoint: /appointment/{id})- dohvaćanje termina pregleda određenog pacijenta
  - GET metoda (endpoint: /doctor/{doctorId}/appointment/{id}) - dohvaćanje svih termina u određenom danu za određenog doktora
  
  S POST i PUT metodama spremamo ili ažuriramo objekte koji se nalaze u cache-u, s GET metodom pristupamo tim objektima, a s DELETE metodom brišemo objekte iz cache-a.

* Cache (Redis Cache): generira se Key,a u Value se sprema objekt formata JSON s sljedećim atributima: AppointmentId, DoctorId, Patient, StartTime, EndTime, RealEndTime, AppointmentStatus. Također kreira se SortedSet određenog imena u kojem se nalaze svi termini određenog doktora te sortiraju se uzlazno početna vremena tih termina. Osim ovog seta, kreira se još jedan Set u kojeg se spremaju svi doktori.

* SQL baza: u bazu se spremaju termini koji su obavljeni te također SQL baza služi za pohranu i korištenje Hangfire job-ova.

* RabbitMQ: na RabbitMQ se šalju eventi koji se okidaju pri svakom pomicanju termina.

* Hangfire: zadaju se trvi vrste job-a na Hangfire: 
   - HangfireJobForCache: job za preraspodjelu termina ukoliko dođe do kašnjenja. Ovaj job je tipa Reccuring Job te se on pokreće svako N vremena (npr. 1 minuta) 
   - HangfireJobForDatabase: job za perzistenciju u bazu je tipa Fire-and-Forget Job i zadaje se kada korisnik spremi završno vrijeme termina i postavi status termina na DONE,
   - HangfireForEventSender: job za slanje eventa je tipa Fire-and-Forget Job i zadaje se pri pomicanju termina





