# BallotGenerator

## What is that? 

Ballot Generator is a simple application to create fraud resistant ballots and was developed as part of a student election in Germany.
It can help you create easily a huge number of ballots with a unique barcode.
The barcode represents a large random number.
The fraud protection comes from the aspect, that only you know all valid numbers. 

## Sounds complex, what I need to know? 

You need to know a bit about LaTeX.
Ballot Generator needs a LaTeX Distribution to compute the ballot to a pdf (for instance).
You don't need to be a LaTeX Geek, if you’re lucky you just need to change some names in the template. 

## How it works?

The functionality of Ballot Generator is very simple.
In the current version it reads a list of random numbers.
The project provides some demo values from random.org.
In the first step each line is transformed into a barcode.
In a second step a LaTeX template get a single barcode as parameter.
In the last step each LaTeX file is computed into a pdf. 

## Validation?

To validate the barcode I have also written an [application](https://github.com/DennisRippinger/VoteChecker/) to check if a ballot is valid.
At the moment the labels are hard coded German.
