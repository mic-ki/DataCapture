#!/bin/bash

# Skript pro aktualizaci souborů v Domain/Core s komentáři

# Cesta k adresáři Domain/Core
CORE_DIR="Domain/Core"

# Seznam souborů k aktualizaci
FILES=(
    "BaseEntity.cs"
    "BaseAuditableEntity.cs"
    "BaseSoftDeleteEntity.cs"
    "BaseAuditableSoftDeleteEntity.cs"
    "DomainEvent.cs"
    "IEntity.cs"
)

# Aktualizace souborů
for file in "${FILES[@]}"; do
    commented_file="${file%.*}_commented.${file##*.}"
    if [ -f "$CORE_DIR/$commented_file" ]; then
        echo "Aktualizuji $file..."
        mv "$CORE_DIR/$commented_file" "$CORE_DIR/$file"
    else
        echo "Soubor $commented_file nenalezen."
    fi
done

echo "Aktualizace dokončena."
