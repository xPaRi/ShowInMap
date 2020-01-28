# ShowInMap
Translate JTSK or S42 to WGS84 and show in map in internet browser.

### About
The application translates geographical coordinates from S-JTSK 2065, S-JTSK 5514 or S-42 to WGS84 and posible show in one of map servers (Bing, Google, OSM, Seznam).

### Warning
The algorithms used were strongly optimized and tested only for use in the Czech Republic. I do not recommend using for coordinates outside of them.

Algorithms were tested on [DOPNUL](https://kgm.zcu.cz/studium/gen1/html/ch03s02.html) (Network densifying the NULRAD measured by GNSS technology) points.

### Syntax
```
ShowInMap <source type> <X> <Y> [B|G|O|S]

 Source type
  JTSK2065       - EPSG 2065 S-JTSK/Krovak South-West, positive coordinates
  JTSK5514       - EPSG 5514 S-JTSK/Krovak East-North, negative coordinates
  S42            - EPSG 28403 Pulkovo 1942/Gauss-Krüger zone 3

 X, Y            - Source coordinates. Automatic decimal point replacement.
                   Example: -820800,60 -1068738,00 -> -820800.60 -1068738.00

 B               - Open internet browser and show coordinates in Bing maps
 G               - Open internet browser and show coordinates in Google maps
 O               - Open internet browser and show coordinates in Open Street Map
 S               - Open internet browser and show coordinates in Seznam maps
```

### Example
```
 ShowInMap JTSK5514 -820800.60 -1068738.00 G
```
### Installation
Copy ```ShowInMap.exe``` from ```install``` path to your own directory.