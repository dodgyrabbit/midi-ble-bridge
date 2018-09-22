from __future__ import print_function
import random

import mido

def main():
    """Parse a note_on msg and compare it to one created with Message()."""

    #message = mido.parse(b'\x90\x4c\x20')

    messages = mido.parse_all(b'\xB5\x8D\x80\x15\x40')
    for message in messages:
        print(message)


if __name__ == '__main__':
    main()
