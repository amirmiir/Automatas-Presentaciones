class CajaRegistradora:
    def __init__(self, cajaChica):
        self.cajaChica = cajaChica
    
    def getSaldoActual(self):
        return self.cajaChica

    def aceptarMonto(self, monto):
        self.cajaChica += monto  

class TipoDispensador:
    def __init__(self, nroProductos, costo):
        self.nroProductos = max(0, nroProductos)
        self.costo = max(0, costo)

    def getNroProductos(self):
        return self.nroProductos

    def getCosto(self):
        return self.costo

    def hacerVenta(self):
        if self.nroProductos > 0:
            self.nroProductos -= 1
        else:
            print("El dispensador está vacío")


def mostrarSeleccion(dispensador1, dispensador2):
    print("Para elegir un producto, ingrese:")
    print(f"G para Galletas (Disponibles: {dispensador1.getNroProductos()}, Costo: {dispensador1.getCosto()})")
    print(f"C para Chocolates (Disponibles: {dispensador2.getNroProductos()}, Costo: {dispensador2.getCosto()})")
    print("-1 para salir")



def venderProducto(caja, dispensador, montoIngresado):
    if montoIngresado >= 20:
        dispensador.hacerVenta()  
        caja.aceptarMonto(20) 
    else:
        print("Monto insuficiente para comprar este producto.")



def pruebaDispensador():
    caja = CajaRegistradora(0)
    dispensadorGalletas = TipoDispensador(20, 20)
    dispensadorChocolates = TipoDispensador(20, 20)
    while True:
        mostrarSeleccion(dispensadorGalletas, dispensadorChocolates)
        montoIngresado = 0
        while montoIngresado < 20:
            monto = float(input("Ingrese dinero (acepta monedas de 5c, 10c, 25c): "))
            if monto in [5, 10, 25]:
                montoIngresado += monto
            elif monto == -1:
                return
            else:
                print("Ingrese una moneda válida.")
                
        vuelto = montoIngresado - 20
        if vuelto > 0:
            print(f"Vuelto: {vuelto}")
            
        while True:
            seleccion = input("Ingrese el producto, por favor(G o C): ")
            seleccion = seleccion.upper()
            if seleccion.upper() in ['G', 'C']:
                break
            else:
                print("Ingrese un producto válido: ")

        print("Recoja su ", end = '')
        if seleccion == 'G':
            print("Galleta", end = '')
            venderProducto(caja, dispensadorGalletas, montoIngresado)
        elif seleccion == 'C':
            print("Chocolate", end = '')
            venderProducto(caja, dispensadorChocolates, montoIngresado)
        print(" al fondo y buen provecho.")


pruebaDispensador()
