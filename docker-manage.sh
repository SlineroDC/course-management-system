#!/bin/bash

# Script para gestionar la aplicación en Docker
# Uso: ./docker-manage.sh <comando>

set -e

BLUE='\033[0;34m'
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Funciones
print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

# Comando: start
cmd_start() {
    print_header "🚀 Iniciando servicios"
    
    print_warning "Construyendo imágenes..."
    docker-compose build
    
    print_warning "Levantando contenedores..."
    docker-compose up -d
    
    print_warning "Esperando a que la BD esté lista..."
    sleep 15
    
    print_warning "Ejecutando migraciones..."
    docker-compose exec -T api dotnet ef database update \
        --project Infrastructure \
        --startup-project API
    
    print_success "¡Servicios iniciados!"
    cmd_status
}

# Comando: stop
cmd_stop() {
    print_header "⏹️  Deteniendo servicios"
    docker-compose down
    print_success "Servicios detenidos"
}

# Comando: status
cmd_status() {
    print_header "📊 Estado de servicios"
    docker-compose ps
    
    echo ""
    print_success "URLs de acceso:"
    echo -e "${BLUE}Frontend:${NC}   http://localhost:5173"
    echo -e "${BLUE}Backend API:${NC}  http://localhost:7260/api"
    echo -e "${BLUE}Swagger:${NC}      http://localhost:7260/swagger"
}

# Comando: logs
cmd_logs() {
    print_header "📋 Logs"
    docker-compose logs -f "${1:-api}"
}

# Comando: test-login
cmd_test_login() {
    print_header "🔐 Probando Login"
    
    local EMAIL="${1:-test@example.com}"
    local PASSWORD="${2:-Test@123456}"
    
    print_warning "Registrando usuario: $EMAIL"
    REGISTER_RESPONSE=$(curl -s -X POST http://localhost:7260/api/auth/register \
        -H "Content-Type: application/json" \
        -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}")
    
    echo "Respuesta: $REGISTER_RESPONSE"
    echo ""
    
    print_warning "Iniciando sesión..."
    LOGIN_RESPONSE=$(curl -s -X POST http://localhost:7260/api/auth/login \
        -H "Content-Type: application/json" \
        -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}")
    
    TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)
    
    if [ -z "$TOKEN" ]; then
        print_error "Login fallido"
        echo "Respuesta: $LOGIN_RESPONSE"
        return 1
    fi
    
    print_success "Login exitoso!"
    echo -e "${BLUE}Token:${NC} ${TOKEN:0:50}..."
}

# Comando: migrate
cmd_migrate() {
    print_header "🔄 Ejecutando migraciones"
    docker-compose exec -T api dotnet ef database update \
        --project Infrastructure \
        --startup-project API
    print_success "Migraciones completadas"
}

# Comando: db-shell
cmd_db_shell() {
    print_header "🗄️  Accediendo a PostgreSQL"
    docker-compose exec db psql -U postgres -d course_db
}

# Comando: api-shell
cmd_api_shell() {
    print_header "🔧 Accediendo a contenedor API"
    docker-compose exec api /bin/bash
}

# Comando: clean
cmd_clean() {
    print_header "🧹 Limpiando todo (CUIDADO - borra datos)"
    print_warning "Esto eliminará todos los contenedores y volúmenes"
    read -p "¿Estás seguro? (s/n): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Ss]$ ]]; then
        docker-compose down -v
        print_success "Limpieza completada"
    else
        print_warning "Operación cancelada"
    fi
}

# Comando: rebuild
cmd_rebuild() {
    print_header "🔨 Reconstruyendo todo"
    cmd_clean
    cmd_start
}

# Comando: health-check
cmd_health_check() {
    print_header "❤️  Health Check"
    
    echo -e "${BLUE}Verificando API...${NC}"
    if curl -f -s http://localhost:7260/api/courses > /dev/null 2>&1; then
        print_success "API está funcionando"
    else
        print_error "API no responde"
    fi
    
    echo -e "${BLUE}Verificando BD...${NC}"
    if docker-compose exec -T db pg_isready -U postgres > /dev/null 2>&1; then
        print_success "BD está funcionando"
    else
        print_error "BD no responde"
    fi
    
    echo -e "${BLUE}Verificando Frontend...${NC}"
    if curl -f -s http://localhost:5173/index.html > /dev/null 2>&1; then
        print_success "Frontend está funcionando"
    else
        print_error "Frontend no responde"
    fi
}

# Comando: help
cmd_help() {
    cat << EOF
${BLUE}Gestor de Docker - Course Management System${NC}

${YELLOW}Uso:${NC}
  ./docker-manage.sh <comando> [opciones]

${YELLOW}Comandos:${NC}
  start           - Inicia todos los servicios (build + up + migrate)
  stop            - Detiene todos los servicios
  status          - Muestra estado de servicios
  logs [servicio] - Ver logs (api, db, frontend) - default: api
  test-login [email] [password] - Prueba login
  migrate         - Ejecuta migraciones de BD
  db-shell        - Accede a PostgreSQL
  api-shell       - Accede a contenedor API
  clean           - Limpia todo (borra datos)
  rebuild         - Limpia y reconstruye todo
  health-check    - Verifica salud de servicios
  help            - Muestra esta ayuda

${YELLOW}Ejemplos:${NC}
  ./docker-manage.sh start
  ./docker-manage.sh logs api
  ./docker-manage.sh test-login test@example.com Test@123456
  ./docker-manage.sh db-shell

${YELLOW}URLs:${NC}
  Frontend:   http://localhost:5173
  Backend:    http://localhost:7260/api
  Swagger:    http://localhost:7260/swagger

EOF
}

# Main
case "${1:-help}" in
    start)
        cmd_start
        ;;
    stop)
        cmd_stop
        ;;
    status)
        cmd_status
        ;;
    logs)
        cmd_logs "$2"
        ;;
    test-login)
        cmd_test_login "$2" "$3"
        ;;
    migrate)
        cmd_migrate
        ;;
    db-shell)
        cmd_db_shell
        ;;
    api-shell)
        cmd_api_shell
        ;;
    clean)
        cmd_clean
        ;;
    rebuild)
        cmd_rebuild
        ;;
    health-check)
        cmd_health_check
        ;;
    help)
        cmd_help
        ;;
    *)
        print_error "Comando desconocido: $1"
        cmd_help
        exit 1
        ;;
esac
