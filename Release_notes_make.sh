#!/bin/sh

fmt -s Release_notes_src.txt |perl -p -e 's/\n/\r\n/' >"Release Notes.txt"

echo "WinSCP-ben binary modban kell feltolteni!"
