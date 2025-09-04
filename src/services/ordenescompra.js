// Importe de rutas para apis, funciones y utilidades
import {
    ORDENES_COMPRA_API,
    ORDENES_COMPRA_GET_BY_ID_API,
    ORDENES_COMPRA_RUBRO_API,
    ORDENES_COMPRA_COMPLETAS_API,
    ORDENES_COMPRA_TOTAL_RUBRO_API,
    ORDENES_COMPRA_RANGO_FECHAS_API,
    ORDENES_COMPRA_REPORTE_PROYECTO_API,
    ORDENES_COMPRA_BALANCE_RUBRO_API,
    ORDENES_COMPRA_INACTIVAR_RUBRO_API,
    RUBROS_COMPLETOS_API, // Para cargar rubros en el select
    PROYECTOS_API // Para cargar proyectos en el select
} from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Variables globales para almacenar rubros y proyectos
let rubrosDisponibles = [];
let proyectosDisponibles = [];

// Obtener todas las órdenes de compra y mostrar
const obtenerTodasLasOrdenesDeCompra = async () => {
    try {
        const response = await getData(ORDENES_COMPRA_COMPLETAS_API);

        if (response && response.data) {
            $('#tablaOrdenesCompra').DataTable({
                destroy: true,
                data: response.data,
                columns: [
                    { data: "id" },
                    { 
                        data: "monto",
                        render: function(data) {
                            return data ? `Q ${parseFloat(data).toFixed(2)}` : 'Q 0.00';
                        }
                    },
                    { 
                        data: "fechaOrden",
                        render: function(data) {
                            return data ? new Date(data).toLocaleDateString('es-GT') : 'N/A';
                        }
                    },
                    { 
                        data: "codigoRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "nombreRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "codigoProyecto",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "nombreProyecto",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "active",
                        render: function(data) {
                            return data === true || data === 1 ? 
                                '<span class="badge bg-success">Activo</span>' : 
                                '<span class="badge bg-danger">Inactivo</span>';
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return `
                                <div class="btn-group" role="group">
                                    <button onclick="editarOrdenCompra(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-warning btn-sm">Editar</button>
                                    <button onclick="eliminarOrdenCompra(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-danger btn-sm">Eliminar</button>
                                </div>
                            `;
                        }
                    }
                ],
                dom: 'Bfrtip',
                buttons: [
                    { extend: 'copy', className: 'btn btn-secondary', text: '<i class="fas fa-copy"></i> Copiar' },
                    { extend: 'excel', className: 'btn btn-secondary', text: '<i class="fas fa-file-excel"></i> Excel' },
                    { extend: 'pdf', className: 'btn btn-secondary', text: '<i class="fas fa-file-pdf"></i> PDF' },
                    { extend: 'print', className: 'btn btn-secondary', text: '<i class="fas fa-print"></i> Imprimir' }
                ],
                language: {
                    // Solución para el error de CORS - usar configuración manual en español
                    "decimal": "",
                    "emptyTable": "No hay datos disponibles",
                    "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
                    "infoEmpty": "Mostrando 0 a 0 de 0 registros",
                    "infoFiltered": "(filtrado de _MAX_ registros totales)",
                    "infoPostFix": "",
                    "thousands": ",",
                    "lengthMenu": "Mostrar _MENU_ registros",
                    "loadingRecords": "Cargando...",
                    "processing": "Procesando...",
                    "search": "Buscar:",
                    "zeroRecords": "No se encontraron registros coincidentes",
                    "paginate": {
                        "first": "Primero",
                        "last": "Último",
                        "next": "Siguiente",
                        "previous": "Anterior"
                    },
                    "aria": {
                        "sortAscending": ": activar para ordenar columna ascendente",
                        "sortDescending": ": activar para ordenar columna descendente"
                    }
                },
                responsive: true,
                order: [[0, 'desc']],
                pageLength: 10,
                lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]],
                columnDefs: [
                    { width: "5%", targets: 0 },
                    { width: "10%", targets: 1 },
                    { width: "10%", targets: 2 },
                    { width: "10%", targets: 3 },
                    { width: "15%", targets: 4 },
                    { width: "10%", targets: 5 },
                    { width: "15%", targets: 6 },
                    { width: "5%", targets: 7 },
                    { width: "15%", targets: 8 }
                ]
            });
        } else {
            showAlert("No se encontraron órdenes de compra.", "warning");
            $('#tablaOrdenesCompra').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    { extend: 'copy', className: 'btn btn-secondary' },
                    { extend: 'excel', className: 'btn btn-secondary' },
                    { extend: 'pdf', className: 'btn btn-secondary' },
                    { extend: 'print', className: 'btn btn-secondary' }
                ],
                language: {
                    // Configuración manual en español para tabla vacía también
                    "decimal": "",
                    "emptyTable": "No hay datos disponibles",
                    "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
                    "infoEmpty": "Mostrando 0 a 0 de 0 registros",
                    "infoFiltered": "(filtrado de _MAX_ registros totales)",
                    "infoPostFix": "",
                    "thousands": ",",
                    "lengthMenu": "Mostrar _MENU_ registros",
                    "loadingRecords": "Cargando...",
                    "processing": "Procesando...",
                    "search": "Buscar:",
                    "zeroRecords": "No se encontraron registros coincidentes",
                    "paginate": {
                        "first": "Primero",
                        "last": "Último",
                        "next": "Siguiente",
                        "previous": "Anterior"
                    }
                }
            });
        }
    } catch (error) {
        showError(error.message, "danger");
        throw error;
    }
};

// Función para cargar rubros en el select
const cargarRubrosEnSelect = async () => {
    try {
        const response = await getData(RUBROS_COMPLETOS_API);
        
        if (response && response.data) {
            rubrosDisponibles = response.data;
            const select = document.getElementById('rubroSelectOrden'); // Cambiado a rubroSelectOrden
            
            if (!select) {
                console.error('Elemento rubroSelectOrden no encontrado');
                return;
            }
            
            // Limpiar opciones excepto la primera
            while (select.options.length > 1) {
                select.remove(1);
            }
            
            // Agregar rubros al select
            rubrosDisponibles.forEach(rubro => {
                const option = document.createElement('option');
                option.value = rubro.id;
                option.textContent = `${rubro.codigo} - ${rubro.nombre}`;
                option.setAttribute('data-codigo', rubro.codigo);
                option.setAttribute('data-nombre', rubro.nombre);
                option.setAttribute('data-codigoProyecto', rubro.codigoProyecto);
                option.setAttribute('data-nombreProyecto', rubro.nombreProyecto);
                option.setAttribute('data-idProyecto', rubro.idProyecto);
                select.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error al cargar rubros:', error);
        showError('No se pudieron cargar los rubros');
    }
};

// Función para cargar proyectos en el select
const cargarProyectosEnSelect = async () => {
    try {
        const response = await getData(PROYECTOS_API);
        
        if (response && response.data) {
            proyectosDisponibles = response.data;
            const select = document.getElementById('proyectoSelect');
            
            if (!select) {
                console.error('Elemento proyectoSelect no encontrado');
                return;
            }
            
            // Limpiar opciones excepto la primera
            while (select.options.length > 1) {
                select.remove(1);
            }
            
            // Agregar proyectos al select
            proyectosDisponibles.forEach(proyecto => {
                const option = document.createElement('option');
                option.value = proyecto.id;
                option.textContent = `${proyecto.codigo} - ${proyecto.nombre}`;
                option.setAttribute('data-codigo', proyecto.codigo);
                option.setAttribute('data-nombre', proyecto.nombre);
                select.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error al cargar proyectos:', error);
        showError('No se pudieron cargar los proyectos');
    }
};

// Función para autocompletar campos cuando se selecciona un rubro
const autocompletarCamposRubro = (rubroId) => {
    const rubro = rubrosDisponibles.find(r => r.id == rubroId);
    
    if (rubro) {
        // Campos de rubro
        if (document.getElementById('codigoRubro')) {
            document.getElementById('codigoRubro').value = rubro.codigo || '';
        }
        if (document.getElementById('nombreRubro')) {
            document.getElementById('nombreRubro').value = rubro.nombre || '';
        }
        
        // Campos de proyecto
        if (document.getElementById('codigoProyecto')) {
            document.getElementById('codigoProyecto').value = rubro.codigoProyecto || '';
        }
        if (document.getElementById('nombreProyecto')) {
            document.getElementById('nombreProyecto').value = rubro.nombreProyecto || '';
        }
        
        // Seleccionar automáticamente el proyecto correspondiente
        if (rubro.idProyecto && document.getElementById('proyectoSelect')) {
            document.getElementById('proyectoSelect').value = rubro.idProyecto;
        }
    } else {
        // Limpiar campos si no se encuentra el rubro
        const campos = ['codigoRubro', 'nombreRubro', 'codigoProyecto', 'nombreProyecto'];
        campos.forEach(campo => {
            if (document.getElementById(campo)) {
                document.getElementById(campo).value = '';
            }
        });
    }
};

// Función para autocompletar campos cuando se selecciona un proyecto
const autocompletarCamposProyecto = (proyectoId) => {
    const proyecto = proyectosDisponibles.find(p => p.id == proyectoId);
    
    if (proyecto) {
        if (document.getElementById('codigoProyecto')) {
            document.getElementById('codigoProyecto').value = proyecto.codigo || '';
        }
        if (document.getElementById('nombreProyecto')) {
            document.getElementById('nombreProyecto').value = proyecto.nombre || '';
        }
    } else {
        // Limpiar campos si no se encuentra el proyecto
        if (document.getElementById('codigoProyecto')) {
            document.getElementById('codigoProyecto').value = '';
        }
        if (document.getElementById('nombreProyecto')) {
            document.getElementById('nombreProyecto').value = '';
        }
    }
};

// Función global para editar orden de compra
window.editarOrdenCompra = async (ordenCompra) => {
    try {
        const response = await getData(ORDENES_COMPRA_GET_BY_ID_API(ordenCompra.id));
        
        if (response && response.data) {
            const ordenCompleta = response.data;
            
            if (document.getElementById('ordenCompraModalLabel')) {
                document.getElementById('ordenCompraModalLabel').textContent = 'Editar Órden de Compra';
            }
            
            document.getElementById('ordenCompraId').value = ordenCompleta.id || '';
            document.getElementById('montoOrden').value = ordenCompleta.monto || '';
            
            // Formatear fecha para datetime-local
            if (ordenCompleta.fechaOrden) {
                const fecha = new Date(ordenCompleta.fechaOrden);
                const fechaFormateada = fecha.toISOString().slice(0, 16);
                document.getElementById('fechaOrden').value = fechaFormateada;
            }
            
            document.getElementById('activeOrden').checked = ordenCompleta.active === true || ordenCompleta.active === 1;
            
            // Cargar rubros y proyectos, luego seleccionar los correspondientes
            await cargarRubrosEnSelect();
            await cargarProyectosEnSelect();
            
            document.getElementById('rubroSelectOrden').value = ordenCompleta.idRubro || ''; // Cambiado a rubroSelectOrden
            autocompletarCamposRubro(ordenCompleta.idRubro);
            
            const modal = new bootstrap.Modal(document.getElementById('modalOrdenCompra'));
            modal.show();
        } else {
            showError('No se pudieron obtener los datos de la orden de compra');
        }
    } catch (error) {
        showError('Error al cargar datos de la orden de compra: ' + error.message);
    }
};

// Función global para eliminar orden de compra
window.eliminarOrdenCompra = (ordenCompra) => {
    showConfirmation(
        `¿Estás seguro de eliminar la orden de compra #${ordenCompra.id} por Q ${ordenCompra.monto || '0.00'}?`,
        "Sí, eliminar",
        "Cancelar"
    ).then(async (confirmado) => {
        if (confirmado) {
            try {
                const response = await deleteData(`${ORDENES_COMPRA_API}/${ordenCompra.id}`);
                
                if (response && response.success) {
                    showSuccess('Orden de compra eliminada correctamente');
                    await obtenerTodasLasOrdenesDeCompra();
                } else {
                    showError(response?.message || 'Error al eliminar la orden de compra');
                }
            } catch (error) {
                showError('Error al eliminar la orden de compra: ' + error.message);
            }
        }
    });
};

// Función para guardar orden de compra
window.guardarOrdenCompra = async () => {
    const form = document.getElementById('formOrdenCompra');
    
    if (!form) {
        showError('Formulario no encontrado');
        return;
    }
    
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    try {
        const ordenCompraId = document.getElementById('ordenCompraId').value;
        const idRubro = document.getElementById('rubroSelectOrden').value; // Cambiado a rubroSelectOrden
        const isEdit = ordenCompraId > 0;
        
        if (!idRubro) {
            showError('Debe seleccionar un rubro');
            return;
        }
        
        const rubroSeleccionado = rubrosDisponibles.find(r => r.id == idRubro);
        
        const ordenCompraData = {
            id: ordenCompraId || 0,
            idRubro: parseInt(idRubro),
            monto: parseFloat(document.getElementById('montoOrden').value),
            fechaOrden: document.getElementById('fechaOrden').value,
            active: document.getElementById('activeOrden').checked,
            codigoRubro: rubroSeleccionado?.codigo || '',
            nombreRubro: rubroSeleccionado?.nombre || '',
            codigoProyecto: rubroSeleccionado?.codigoProyecto || '',
            nombreProyecto: rubroSeleccionado?.nombreProyecto || ''
        };
        
        let response;
        if (isEdit) {
            response = await putData(`${ORDENES_COMPRA_API}/${ordenCompraId}`, ordenCompraData);
        } else {
            response = await postData(ORDENES_COMPRA_API, ordenCompraData);
        }
        
        if (response && response.success) {
            showSuccess(response.message || (isEdit ? 'Orden de compra actualizada' : 'Orden de compra creada'));
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalOrdenCompra'));
            if (modal) {
                modal.hide();
            }
            await obtenerTodasLasOrdenesDeCompra();
        } else {
            showError(response?.message || 'Error al guardar la orden de compra');
        }
    } catch (error) {
        showError('Error al guardar la orden de compra: ' + error.message);
    }
};

// Función para configurar event listeners de forma segura
const configurarEventListeners = () => {
    const btnGuardar = document.getElementById('btnGuardarOrdenCompra');
    const rubroSelect = document.getElementById('rubroSelectOrden'); // Cambiado a rubroSelectOrden
    const proyectoSelect = document.getElementById('proyectoSelect');
    const modal = document.getElementById('modalOrdenCompra');
    
    if (btnGuardar) {
        btnGuardar.addEventListener('click', window.guardarOrdenCompra);
    } else {
        console.error('Botón btnGuardarOrdenCompra no encontrado');
    }
    
    if (rubroSelect) {
        rubroSelect.addEventListener('change', function() {
            autocompletarCamposRubro(this.value);
        });
    }
    
    if (proyectoSelect) {
        proyectoSelect.addEventListener('change', function() {
            autocompletarCamposProyecto(this.value);
        });
    }
    
    if (modal) {
        modal.addEventListener('show.bs.modal', async function () {
            if (!document.getElementById('ordenCompraId').value) {
                document.getElementById('ordenCompraModalLabel').textContent = 'Nueva Órden de Compra';
                
                try {
                    // Cargar rubros y proyectos
                    await cargarRubrosEnSelect();
                    await cargarProyectosEnSelect();
                    
                    // Establecer fecha actual por defecto
                    document.getElementById('fechaOrden').value = new Date().toISOString().slice(0, 16);
                    
                    // Limpiar campos
                    document.getElementById('rubroSelectOrden').value = ''; // Cambiado a rubroSelectOrden
                    document.getElementById('proyectoSelect').value = '';
                    autocompletarCamposRubro('');
                    autocompletarCamposProyecto('');
                } catch (error) {
                    console.error('Error al inicializar modal:', error);
                }
            }
        });
        
        modal.addEventListener('hidden.bs.modal', function () {
            document.getElementById('formOrdenCompra').reset();
            document.getElementById('ordenCompraId').value = '';
            document.getElementById('rubroSelectOrden').value = ''; // Cambiado a rubroSelectOrden
            document.getElementById('proyectoSelect').value = '';
            autocompletarCamposRubro('');
            autocompletarCamposProyecto('');
        });
    }
};

// Configurar eventos cuando el DOM esté completamente cargado
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', configurarEventListeners);
} else {
    configurarEventListeners();
}

// Inicializar
window.onload = async () => {
    await obtenerTodasLasOrdenesDeCompra();
};