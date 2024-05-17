-- Procedura do tworzenia drzewa 
-- procedura do dodawania osob
-- jesli osoba nalezy do rodziny - jest wezlem
create database GDrzewo;
use GDrzewo;
GO
DROP TABLE  IF EXISTS GDrzewo; 
GO
-- do tego bedzie funkcja ale narazie tworzymy baze danych
-- uwaga w destruktorze glownej funkcji trzeba bedzie zaimplementowac usuwanie tabeli!! wraz z danymi!!!	
DROP PROCEDURE   IF EXISTS StworzDrzewo; 
GO
CREATE PROCEDURE StworzDrzewo
AS
	BEGIN
		DROP TABLE  IF EXISTS GDrzewo; 
		CREATE TABLE GDrzewo  
		(  
			CzlonekId hierarchyid PRIMARY KEY,  
			Poziom AS CzlonekId.GetLevel(),  
			Imie nvarchar(20) NOT NULL,
			Nazwisko nvarchar(20) NOT NULL,
			ImieMalzonka nvarchar(20),
			NazwiskoMalzonka nvarchar(20),
			DataUrodzenia DATE NOT NULL,
			DataSmierci DATE
	    ) ;  
	  -- do przesukiwania wszerz
	CREATE INDEX GDrzewo_BreadthFirstIndex ON GDrzewo(CzlonekId,Poziom); 
	--do szukania osob o takich imionach i nazwiskach
	CREATE INDEX GDrzewo_Search ON GDrzewo(CzlonekId, Imie, Nazwisko);
	END;
GO
	-- procedura dodawania osoby, wg rodzica - uwaga moze byc NULL wtedy jest to poczatek drzewa
DROP PROCEDURE IF EXISTS DodajCzlonka; 
GO
CREATE PROCEDURE DodajCzlonka( @ImieRodzica nvarchar(20) = NULL, @NazwiskoRodzica nvarchar(20)=NULL, @ImieMalzonka nvarchar(20) = NULL, @NazwiskoMalzonka nvarchar(20)=NULL,
@Imie nvarchar(20),@Nazwisko nvarchar(20),@DataUrodzenia DATE,@DataSmierci DATE = NULL)   
AS  
BEGIN  
    DECLARE @rodzic_id hierarchyid;
	DECLARE @last_child hierarchyid;
INS_EMP:
	IF @ImieRodzica is not NULL
		BEGIN
		SELECT @rodzic_id = CzlonekId FROM GDrzewo WHERE Imie=@ImieRodzica and Nazwisko = @NazwiskoRodzica;
		IF @rodzic_id IS NOT NULL
			BEGIN
				SELECT 'ff';
				SELECT @last_child = MAX(CzlonekId) FROM GDrzewo   
					WHERE CzlonekId.GetAncestor(1) = @rodzic_id;
				INSERT INTO GDrzewo (CzlonekId, Imie, Nazwisko,ImieMalzonka,NazwiskoMalzonka,DataUrodzenia,DataSmierci)  
					SELECT @rodzic_id.GetDescendant(@last_child, NULL), @Imie, @Nazwisko,@ImieMalzonka,@NazwiskoMalzonka,@DataUrodzenia,@DataSmierci;
			END
		ELSE
		  BEGIN
			  IF (@ImieMalzonka is not NULL) 
				  SELECT @last_child = CzlonekId FROM GDrzewo WHERE Imie=@ImieMalzonka and Nazwisko = @Nazwisko;
				  IF @last_child IS NOT NULL
					  INSERT INTO GDrzewo (CzlonekId, Imie, Nazwisko,ImieMalzonka,NazwiskoMalzonka,DataUrodzenia,DataSmierci)  
								SELECT @rodzic_id.GetDescendant(@last_child, NULL), @Imie, @Nazwisko,@ImieMalzonka,@NazwiskoMalzonka,@DataUrodzenia,@DataSmierci;
				  ELSE
					 THROW 51000, 'Czlonek nie jest z nikim spokrewniony.', 1;
	      END
	    END
	ELSE
		DECLARE @myCursor CURSOR;
		
		SET @myCursor = CURSOR LOCAL SCROLL FOR  SELECT CzlonekId FROM  GDrzewo ;
		OPEN  @myCursor;

		FETCH FIRST FROM @myCursor INTO @rodzic_id;

		DEALLOCATE @myCursor;
		
		IF (@rodzic_id is NULL)
		BEGIN
			INSERT INTO GDrzewo (CzlonekId, Imie, Nazwisko,DataUrodzenia, DataSmierci) VALUES(hierarchyid::GetRoot().GetDescendant(NULL,NULL),@Imie,@Nazwisko,@DataUrodzenia,@DataSmierci);
		END
		ELSE
			BEGIN
			  -- mamy malzonka i szukamy jego rodzica
			  SELECT @last_child = CzlonekId FROM GDrzewo WHERE Imie=@ImieMalzonka and Nazwisko = @NazwiskoMalzonka;
			  if @last_child = @rodzic_id
				begin
				  INSERT INTO GDrzewo (CzlonekId, Imie, Nazwisko,ImieMalzonka,NazwiskoMalzonka,DataUrodzenia,DataSmierci)  
								SELECT hierarchyid::GetRoot().GetDescendant(@rodzic_id, NULL), @Imie, @Nazwisko,@ImieMalzonka,@NazwiskoMalzonka,@DataUrodzenia,@DataSmierci;
			     end
			  else IF @last_child IS NOT NULL
				  INSERT INTO GDrzewo (CzlonekId, Imie, Nazwisko,ImieMalzonka,NazwiskoMalzonka,DataUrodzenia,DataSmierci)  
							SELECT @rodzic_id.GetDescendant(@last_child, NULL), @Imie, @Nazwisko,@ImieMalzonka,@NazwiskoMalzonka,@DataUrodzenia,@DataSmierci;
			  ELSE
				 THROW 51000, 'Czlonek nie jest z nikim spokrewniony.', 1;
			END			
IF @@error <> 0 GOTO INS_EMP   
END;  
GO

 
IF OBJECT_ID ('OsobaInsert','TR') IS NOT NULL
   DROP TRIGGER OsobaInsert;
GO
CREATE TRIGGER OsobaInsert ON  GDrzewo
AFTER INSERT  
AS  
BEGIN
	DECLARE @malzonekimie varchar(20);
	DECLARE @malzoneknazwisko varchar(20);
	DECLARE @imiewprowadzone varchar(20);
	DECLARE @nazwiskowprowadzone varchar(20);
	DECLARE @imie varchar(20);
	DECLARE @malzonekid hierarchyid;

SELECT @imiewprowadzone = Imie FROM inserted;
SELECT @nazwiskowprowadzone = Nazwisko FROM inserted;
SELECT @malzonekimie = ImieMalzonka FROM inserted;
SELECT @malzoneknazwisko = NazwiskoMalzonka FROM inserted;
SELECT @malzonekid = CzlonekId, @imie = ImieMalzonka 
FROM GDrzewo 
WHERE Imie = @malzonekimie AND Nazwisko = @malzoneknazwisko;

IF @malzonekid IS NOT NULL and @imie is NULL
	BEGIN
		UPDATE GDrzewo
		SET  ImieMalzonka = @imiewprowadzone, NazwiskoMalzonka = @nazwiskowprowadzone 
		WHERE CzlonekId= @malzonekid;	
	END

END

use GDrzewo;
EXEC StworzDrzewo;
EXEC DodajCzlonka @Imie='Alicja',@Nazwisko='Kaluza',@DataUrodzenia='2022-12-10' ;
EXEC DodajCzlonka @Imie='Alicja',@Nazwisko='Kaluza',@ImieMalzonka='Alicja',@NazwiskoMalzonka='Kaluza',@DataUrodzenia='2022-12-10',@DataSmierci='2023-12-10' ;
SELECT * FROM GDrzewo;
DROP TABLE IF EXISTS GDrzewo;


case

FROM Gdrzewo;
--raporty 1) po prostu select - moze byc z aplikacji
--2) raport nr 2 : wypisz dzieci danej osoby - jakis join- moze byc w c#
---3) znajdz malzonka
--4) zrobienie testow -- poziom aplikacji
SELECT CzlonekId.ToString() as Id,Poziom,Imie,Nazwisko,ImieMalzonka,NazwiskoMalzonka , (YEAR(GETDATE()) - YEAR(DataUrodzenia)) as wiek from GDrzewo WHERE DataSmierci is  NULL;
 SELECT CzlonekId.ToString() as Id,Poziom,Imie,Nazwisko,ImieMalzonka,NazwiskoMalzonka,(YEAR(DataSmierci) - YEAR(DataUrodzenia)) as lata from GDrzewo WHERE DataSmierci is not NULL;