Antes de ingresar al sistema con una cuenta de gmail se deben modificar las siguientes listas:

- Para ingresar como administrador del sistema, se debe agregar la direccion de email en la lista administradores que se encuentra en el controller LoginController.cs
  (dentro de GoogleResponse()) .

- Realizar lo mismo en la lista ﻿allowdemails que se encuentra en el controlador HomeController.cs (dentro de ﻿Privacy()).

Luego de iniciar sesion, se genera un registro en la tabla empleados que contiene en el campo rol la identificacion Administrador . Este rol permite acceder a todas las opciones del
sistema. Caso contrario se accede en modo Conserje, cuyo acceso es mas limitado.
